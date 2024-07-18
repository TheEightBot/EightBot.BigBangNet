using ReactiveUI;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms.PopUp
{
	public class ReactivePopupPage<TViewModel> : PopupPage, IViewFor<TViewModel>
		where TViewModel : class
	{
		/// <summary>
		/// The ViewModel to display
		/// </summary>
		public TViewModel ViewModel {
			get { return (TViewModel)GetValue(ViewModelProperty); }
			set { SetValue(ViewModelProperty, value); }
		}
		public static readonly BindableProperty ViewModelProperty =
			BindableProperty.Create(nameof(ViewModel), typeof(TViewModel), typeof(ReactivePopupPage<TViewModel>), default(TViewModel), BindingMode.OneWay);

		object IViewFor.ViewModel {
			get { return ViewModel; }
			set { ViewModel = (TViewModel)value; }
		}
	}
}