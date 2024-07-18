using System;
using ReactiveUI;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveUI.Fody.Helpers;

namespace EightBot.BigBang.ViewModel
{
	public class SelectionViewModel<TSelectedValKey> : ViewModelBase
	{
        [Reactive]
		public TSelectedValKey Key { get; set; }

        [Reactive]
		public string DisplayValue { get; set; }

        [Reactive]
		public bool Selected { get; set; }


        [Reactive]
        public ReactiveCommand<object, Unit> ToggleSelected { get; private set; }

		public override string Title
		{
			get
			{
				return DisplayValue ?? string.Empty;
			}
		}

        public SelectionViewModel() : base()
        {
        }
        
        public SelectionViewModel(bool registerObservables) : base(registerObservables)
        {
        }

		protected override void RegisterObservables()
		{
            ToggleSelected = 
                ReactiveCommand
                    .Create<object, Unit>(_ => { Selected = !Selected; return Unit.Default; })
                    .DisposeWith(ViewModelBindings);
		}
	}
}
