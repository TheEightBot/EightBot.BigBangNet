using System.Reactive.Disposables;
using System.Runtime.Serialization;
using ReactiveUI;

namespace EightBot.BigBang.ViewModel
{
    public abstract class ActivatableViewModelBase : ViewModelBase, IActivatableViewModel
    {
        public ViewModelActivator Activator { get; } = new ViewModelActivator();

        protected ActivatableViewModelBase() : base(false)
        {
            this.WhenActivated((CompositeDisposable x) =>
            {
                this.ViewModelBindings?.Clear();

                RegisterObservables();
                _bindingsRegistered = true;

                this.ViewModelBindings?.DisposeWith(x);
            });
        }
    }
}
