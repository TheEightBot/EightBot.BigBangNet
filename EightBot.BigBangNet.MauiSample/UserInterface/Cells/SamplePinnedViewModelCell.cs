using System;
using EightBot.BigBang.Sample.Models;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.XamForms;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;
using System.Reactive.Disposables;
using EightBot.BigBang.XamForms.Views;

namespace EightBot.BigBang.SampleApp.UserInterface.Cells
{
    public class SamplePinnedViewModelCell : ViewCellBase<SampleViewModel, SampleModel>
    {
        Label _stringProperty;

        TapGestureRecognizer _tapped;

        protected override void SetupUserInterface()
        {
            _stringProperty = new Label { 
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
            this.OneWayBind(ViewModel, x => x.Data.StringProperty, c => c._stringProperty.Text)
                .DisposeWith(ControlBindings);

            this.OneWayBind(ViewModel, x => x.UnitCommand, c => c._tapped.Command)
                .DisposeWith(ControlBindings);
        }

        protected override void MapDataModelToViewModel(SampleViewModel viewModel, SampleModel dataModel)
        {
            viewModel.Data = dataModel;
        }
    }
}
