using System;
using System.Linq;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using FluentValidation;
using FluentValidation.Results;
using ReactiveUI;
using System.Reactive;
using System.Runtime.Serialization;
using System.Reactive.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Splat;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Concurrency;
using System.ComponentModel;

namespace EightBot.BigBang.ViewModel
{
    public abstract class ValidationViewModelBase<TValidationObject> : ViewModelBase
        where TValidationObject : class
    {
        public static TimeSpan DefaultValidationChangeThrottleDuration = TimeSpan.FromMilliseconds(17 * 4);

        private AbstractValidator<TValidationObject> _cachedValidator;

        protected ObservableAsPropertyHelper<bool> _isValid;
        public bool IsValid
        {
            get { return _isValid != null && _isValid.Value; }
        }

        public ObservableCollection<ValidationInformation> ValidationErrors { get; } = new ObservableCollection<ValidationInformation>();

        public abstract AbstractValidator<TValidationObject> Validator { get; }

        protected AbstractValidator<TValidationObject> GetCachedValidator(Func<AbstractValidator<TValidationObject>> validatorCreation, string validatorName = null)
        {
            if (_cachedValidator != null)
            {
                return _cachedValidator;
            }

            var registeredInstance = Locator.Current.GetService<AbstractValidator<TValidationObject>>();

            if (registeredInstance != default(AbstractValidator<TValidationObject>))
            {
                _cachedValidator = registeredInstance;
                return _cachedValidator;
            }

            var validator = validatorCreation?.Invoke();

            Locator.CurrentMutable.RegisterConstant(validator, typeof(AbstractValidator<TValidationObject>), validatorName);

            _cachedValidator = validator;

            return _cachedValidator;
        }

        protected ValidationViewModelBase() : base()
        {
        }

        protected ValidationViewModelBase(bool registerObservables = true) : base(registerObservables)
        {
        }

        protected IDisposable RegisterValidation<TDoesntMatter>(IObservable<TDoesntMatter> validationTrigger)
        {
            return RegisterValidation(validationTrigger?.Select(_ => Unit.Default), null, null);
        }

        protected IDisposable RegisterValidation(IObservable<Unit> validationTrigger = null, IScheduler observationScheduler = null, TimeSpan? changeThrottleDuration = null)
        {
            var validatorDisposables = new StackCompositeDisposable();

            if (validationTrigger == null)
            {
                validationTrigger =
                    this.WhenAnyObservable(x => x.Changed)
                        .ObserveOn(RxApp.TaskpoolScheduler)
                        .Do(x => this.Log().Debug($"Changed:\t{x.PropertyName}"))
                        .Select(_ => Unit.Default)
                        .StartWith(Unit.Default);
            }

            var validator =
                validationTrigger
                    .ObserveOn(RxApp.TaskpoolScheduler)
                    .ThrottleFirst(changeThrottleDuration ?? DefaultValidationChangeThrottleDuration, RxApp.TaskpoolScheduler)
                    .Select(_ => Observable.FromAsync(token => Validator?.ValidateAsync(this as TValidationObject, token) ?? Task.FromResult(new ValidationResult())))
                    .Switch()
                    .Do(result => this.Log().Debug($"Validated {typeof(TValidationObject).Name} : {result.IsValid} - {string.Join("\t", result.Errors.Select(x => x.ErrorMessage))}"))
                    .Publish().RefCount();

            validator
                .Select(x => x.IsValid)
                .DistinctUntilChanged()
                .ToProperty(this, x => x.IsValid, out _isValid, scheduler: observationScheduler ?? RxApp.MainThreadScheduler)
                .DisposeWith(validatorDisposables);

            validator
                .ObserveOn(RxApp.TaskpoolScheduler)
                .Select(x =>
                    x.Errors
                        .Select(err =>
                            new ValidationInformation(err.PropertyName, err.ErrorMessage, err.AttemptedValue)
                            {
                                ErrorCode = err.ErrorCode
                            })
                        .ToList())
                .ObserveOn(observationScheduler ?? RxApp.MainThreadScheduler)
                .Subscribe(errors =>
                {
                    if (ValidationErrors.Any())
                        ValidationErrors.Clear();

                    if (errors?.Any() ?? false)
                        foreach (var error in errors)
                        {
                            ValidationErrors.Add(error);
                        }
                })
                .DisposeWith(validatorDisposables);


            return validatorDisposables;
        }

        public IObservable<ValidationInformation> MonitorValidationInformationFor<TProperty>(Expression<Func<TValidationObject, TProperty>> property)
        {
            var member = property.Body as MemberExpression;
            var propertyName = member.Member.Name;

            var validInformation = new ValidationInformation(propertyName);

            return
                Observable
                    .Merge(
                        Observable
                            .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                                eventHandler =>
                                {
                                    void Handler(object sender, NotifyCollectionChangedEventArgs e) => eventHandler?.Invoke(e);
                                    return Handler;
                                },
                                x => ValidationErrors.CollectionChanged += x,
                                x => ValidationErrors.CollectionChanged -= x)
                            .ObserveOn(RxApp.TaskpoolScheduler)
                            .SelectMany(x => ValidationErrors?.Where(ni => ni.PropertyName.Equals(propertyName)))
                            .Where(x => x != null),
                        Observable
                            .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                                eventHandler =>
                                {
                                    void Handler(object sender, NotifyCollectionChangedEventArgs e) => eventHandler?.Invoke(e);
                                    return Handler;
                                },
                                x => ValidationErrors.CollectionChanged += x,
                                x => ValidationErrors.CollectionChanged -= x)
                            .ObserveOn(RxApp.TaskpoolScheduler)
                            .Where(x => !ValidationErrors?.Any(ni => ni.PropertyName.Equals(propertyName)) ?? true)
                            .Select(_ => validInformation)
                            .StartWith(validInformation))
                       .DistinctUntilChanged();
        }
    }
}
