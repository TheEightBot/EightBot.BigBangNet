using System;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.Maui;
using ReactiveUI;
using ReactiveUI.Maui;
using Microsoft.Maui;
using System.Reactive.Disposables;
using EightBot.BigBang.Maui.Views;

namespace EightBot.BigBang.SampleApp.UserInterface.Cells
{
    public class SampleViewModelCell : ViewCellBase<SampleViewModel>
    {
        Label _stringProperty;

        TapGestureRecognizer _tapped;

        public SampleViewModelCell()
        {
        }

        protected override void SetupUserInterface()
        {
            _stringProperty = new Label
            {
                Margin = 8,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            _tapped = new TapGestureRecognizer();
            _stringProperty.GestureRecognizers.Add(_tapped);

            this.View = _stringProperty;
        }

        protected override void BindControls()
        {
            this.OneWayBind(ViewModel, x => x.StringProperty, c => c._stringProperty.Text)
                .DisposeWith(ControlBindings);

            this.OneWayBind(ViewModel, x => x.UnitCommand, c => c._tapped.Command)
                .DisposeWith(ControlBindings);
        }
    }
}
