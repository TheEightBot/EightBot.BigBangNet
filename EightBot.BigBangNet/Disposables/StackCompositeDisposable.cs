﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace System.Reactive.Disposables
{
    /// <summary>
    /// Represents a group of disposable resources that are disposed together starting from the most recently added.
    /// </summary>
    [Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Backward compat + ideally want to get rid of the ICollection nature of the type.")]
    public sealed class StackCompositeDisposable : ICollection<IDisposable>, ICancelable
    {
        private readonly object _gate = new object();
        private bool _disposed;
        private List<IDisposable> _disposables;
        private int _count;
        private const int SHRINK_THRESHOLD = 64;

        // Default initial capacity of the _disposables list in case
        // The number of items is not known upfront
        private const int DEFAULT_CAPACITY = 16;

        /// <summary>
        /// Initializes a new instance of the <see cref="StackCompositeDisposable"/> class with no disposables contained by it initially.
        /// </summary>
        public StackCompositeDisposable()
        {
            _disposables = new List<IDisposable>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackCompositeDisposable"/> class with the specified number of disposables.
        /// </summary>
        /// <param name="capacity">The number of disposables that the new CompositeDisposable can initially store.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity"/> is less than zero.</exception>
        public StackCompositeDisposable(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _disposables = new List<IDisposable>(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackCompositeDisposable"/> class from a group of disposables.
        /// </summary>
        /// <param name="disposables">Disposables that will be disposed together.</param>
        /// <exception cref="ArgumentNullException"><paramref name="disposables"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Any of the disposables in the <paramref name="disposables"/> collection is <c>null</c>.</exception>
        public StackCompositeDisposable(params IDisposable[] disposables)
        {
            if (disposables == null)
            {
                throw new ArgumentNullException(nameof(disposables));
            }

            Init(disposables, disposables.Length);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StackCompositeDisposable"/> class from a group of disposables.
        /// </summary>
        /// <param name="disposables">Disposables that will be disposed together.</param>
        /// <exception cref="ArgumentNullException"><paramref name="disposables"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Any of the disposables in the <paramref name="disposables"/> collection is <c>null</c>.</exception>
        public StackCompositeDisposable(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null)
            {
                throw new ArgumentNullException(nameof(disposables));
            }

            // If the disposables is a collection, get its size
            // and use it as a capacity hint for the copy.
            if (disposables is ICollection<IDisposable> c)
            {
                Init(disposables, c.Count);
            }
            else
            {
                // Unknown sized disposables, use the default capacity hint
                Init(disposables, DEFAULT_CAPACITY);
            }
        }

        /// <summary>
        /// Initialize the inner disposable list and count fields.
        /// </summary>
        /// <param name="disposables">The enumerable sequence of disposables.</param>
        /// <param name="capacityHint">The number of items expected from <paramref name="disposables"/></param>
        private void Init(IEnumerable<IDisposable> disposables, int capacityHint)
        {
            var list = new List<IDisposable>(capacityHint);

            // do the copy and null-check in one step to avoid a
            // second loop for just checking for null items
            foreach (var d in disposables)
            {
                if (d == null)
                {
                    throw new ArgumentException("Disposables collection can not contain null values.", nameof(disposables));
                }
                list.Add(d);
            }

            _disposables = list;
            // _count can be read by other threads and thus should be properly visible
            // also releases the _disposables contents so it becomes thread-safe
            Volatile.Write(ref _count, _disposables.Count);
        }

        /// <summary>
        /// Gets the number of disposables contained in the <see cref="StackCompositeDisposable"/>.
        /// </summary>
        public int Count => Volatile.Read(ref _count);

        /// <summary>
        /// Adds a disposable to the <see cref="StackCompositeDisposable"/> or disposes the disposable if the <see cref="StackCompositeDisposable"/> is disposed.
        /// </summary>
        /// <param name="item">Disposable to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public void Add(IDisposable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (_gate)
            {
                if (!_disposed)
                {
                    _disposables.Add(item);
                    // If read atomically outside the lock, it should be written atomically inside
                    // the plain read on _count is fine here because manipulation always happens
                    // from inside a lock.
                    Volatile.Write(ref _count, _count + 1);
                    return;
                }
            }

            item.Dispose();
        }

        /// <summary>
        /// Removes and disposes the first occurrence of a disposable from the <see cref="StackCompositeDisposable"/>.
        /// </summary>
        /// <param name="item">Disposable to remove.</param>
        /// <returns>true if found; false otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public bool Remove(IDisposable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (_gate)
            {
                // this composite was already disposed and if the item was in there
                // it has been already removed/disposed
                if (_disposed)
                {
                    return false;
                }

                //
                // List<T> doesn't shrink the size of the underlying array but does collapse the array
                // by copying the tail one position to the left of the removal index. We don't need
                // index-based lookup but only ordering for sequential disposal. So, instead of spending
                // cycles on the Array.Copy imposed by Remove, we use a null sentinel value. We also
                // do manual Swiss cheese detection to shrink the list if there's a lot of holes in it.
                //

                // read fields as infrequently as possible
                var current = _disposables;

                var i = current.IndexOf(item);
                if (i < 0)
                {
                    // not found, just return
                    return false;
                }

                current[i] = null;

                if (current.Capacity > SHRINK_THRESHOLD && _count < current.Capacity / 2)
                {
                    var fresh = new List<IDisposable>(current.Capacity / 2);

                    foreach (var d in current)
                    {
                        if (d != null)
                        {
                            fresh.Add(d);
                        }
                    }

                    _disposables = fresh;
                }

                // make sure the Count property sees an atomic update
                Volatile.Write(ref _count, _count - 1);
            }

            // if we get here, the item was found and removed from the list
            // just dispose it and report success

            item.Dispose();

            return true;
        }

        /// <summary>
        /// Disposes all disposables in the group and removes them from the group.
        /// </summary>
        public void Dispose()
        {
            var currentDisposables = default(List<IDisposable>);
            lock (_gate)
            {
                if (!_disposed)
                {
                    currentDisposables = _disposables;
                    // nulling out the reference is faster no risk to
                    // future Add/Remove because _disposed will be true
                    // and thus _disposables won't be touched again.
                    _disposables = null;

                    Volatile.Write(ref _count, 0);
                    Volatile.Write(ref _disposed, true);
                }
            }

            if (currentDisposables != null)
            {
                for (int i = currentDisposables.Count - 1; i >= 0; i--)
                {
                    currentDisposables[i]?.Dispose();
                }
            }
        }

        /// <summary>
        /// Removes and disposes all disposables from the <see cref="StackCompositeDisposable"/>, but does not dispose the <see cref="StackCompositeDisposable"/>.
        /// </summary>
        public void Clear()
        {
            var previousDisposables = default(IDisposable[]);
            lock (_gate)
            {
                // disposed composites are always clear
                if (_disposed)
                {
                    return;
                }

                var current = _disposables;

                previousDisposables = current.ToArray();
                current.Clear();

                Volatile.Write(ref _count, 0);
            }

            if(previousDisposables != null)
            {
                for (int i = previousDisposables.Length - 1; i >= 0; i--)
                {
                    previousDisposables[i]?.Dispose();
                }
            }
        }

        /// <summary>
        /// Determines whether the <see cref="StackCompositeDisposable"/> contains a specific disposable.
        /// </summary>
        /// <param name="item">Disposable to search for.</param>
        /// <returns>true if the disposable was found; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="item"/> is <c>null</c>.</exception>
        public bool Contains(IDisposable item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            lock (_gate)
            {
                if (_disposed)
                {
                    return false;
                }
                return _disposables.Contains(item);
            }
        }

        /// <summary>
        /// Copies the disposables contained in the <see cref="StackCompositeDisposable"/> to an array, starting at a particular array index.
        /// </summary>
        /// <param name="array">Array to copy the contained disposables to.</param>
        /// <param name="arrayIndex">Target index at which to copy the first disposable of the group.</param>
        /// <exception cref="ArgumentNullException"><paramref name="array"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than zero. -or - <paramref name="arrayIndex"/> is larger than or equal to the array length.</exception>
        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            if (arrayIndex < 0 || arrayIndex >= array.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }

            lock (_gate)
            {
                // disposed composites are always empty
                if (_disposed)
                {
                    return;
                }

                if (arrayIndex + _count > array.Length)
                {
                    // there is not enough space beyond arrayIndex 
                    // to accommodate all _count disposables in this composite
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex));
                }
                var i = arrayIndex;
                foreach (var d in _disposables)
                {
                    if (d != null)
                    {
                        array[i++] = d;
                    }
                }
            }
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="StackCompositeDisposable"/>.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        public IEnumerator<IDisposable> GetEnumerator()
        {
            lock (_gate)
            {
                if (_disposed || _count == 0)
                {
                    return EMPTY_ENUMERATOR;
                }
                // the copy is unavoidable but the creation
                // of an outer IEnumerable is avoidable
                return new CompositeEnumerator(_disposables.ToArray());
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="StackCompositeDisposable"/>.
        /// </summary>
        /// <returns>An enumerator to iterate over the disposables.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Gets a value that indicates whether the object is disposed.
        /// </summary>
        public bool IsDisposed => Volatile.Read(ref _disposed);

        /// <summary>
        /// An empty enumerator for the <see cref="GetEnumerator"/>
        /// method to avoid allocation on disposed or empty composites.
        /// </summary>
        private static readonly CompositeEnumerator EMPTY_ENUMERATOR =
            new CompositeEnumerator(new IDisposable[0]);

        /// <summary>
        /// An enumerator for an array of disposables.
        /// </summary>
        private sealed class CompositeEnumerator : IEnumerator<IDisposable>
        {
            private readonly IDisposable[] disposables;
            private int index;

            public CompositeEnumerator(IDisposable[] disposables)
            {
                this.disposables = disposables;
                index = -1;
            }

            public IDisposable Current => disposables[index];

            object IEnumerator.Current => disposables[index];

            public void Dispose()
            {
                // Avoid retention of the referenced disposables
                // beyond the lifecycle of the enumerator.
                // Not sure if this happens by default to
                // generic array enumerators though.
                var disposables = this.disposables;
                Array.Clear(disposables, 0, disposables.Length);
            }

            public bool MoveNext()
            {
                var disposables = this.disposables;

                for (; ; )
                {
                    var idx = ++index;
                    if (idx >= disposables.Length)
                    {
                        return false;
                    }
                    // inlined that filter for null elements
                    if (disposables[idx] != null)
                    {
                        return true;
                    }
                }
            }

            public void Reset()
            {
                index = -1;
            }
        }
    }
}