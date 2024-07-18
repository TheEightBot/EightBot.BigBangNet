using System;
using EightBot.BigBang.Sample.ViewModels;
using EightBot.BigBang.Maui.Pages;
using ReactiveUI;
using Microsoft.Maui;
using System.Reactive.Disposables;

namespace EightBot.BigBang.SampleApp.UserInterface.Pages
{
    public class NumericConverterPage : ContentPageBase<SampleViewModel>
    {
        StackLayout _mainContainer;

        Entry
            _emptyStringInt, _allowZeroInt,
            _emptyStringDecimal, _allowZeroDecimal,
            _emptyStringNullableDecimal, _allowZeroNullableDecimal;

        public NumericConverterPage()
        {
            ViewModel = new SampleViewModel();
        }

        protected override void SetupUserInterface()
        {
            _mainContainer = new StackLayout
            {

            };

            _emptyStringInt = new Entry
            {
                Placeholder = "Int - Zeros will be an empty string",
                Keyboard = Keyboard.Numeric
            };

            _allowZeroInt = new Entry
            {
                Placeholder = "Int - Zeros will be zeros",
                Keyboard = Keyboard.Numeric
            };

            _emptyStringDecimal = new Entry
            {
                Placeholder = "Decimal - Zeros will be an empty string",
                Keyboard = Keyboard.Numeric
            };

            _allowZeroDecimal = new Entry
            {
                Placeholder = "Decimal - Zeros will be zeros",
                Keyboard = Keyboard.Numeric
            };

            _emptyStringNullableDecimal = new Entry
            {
                Placeholder = "Decimal? - Zeros will be an empty string",
                Keyboard = Keyboard.Numeric
            };

            _allowZeroNullableDecimal = new Entry
            {
                Placeholder = "Decimal? - Zeros will be zeros",
                Keyboard = Keyboard.Numeric
            };

            _mainContainer.Children.Add(_emptyStringInt);
            _mainContainer.Children.Add(_allowZeroInt);
            _mainContainer.Children.Add(_emptyStringDecimal);
            _mainContainer.Children.Add(_allowZeroDecimal);
            _mainContainer.Children.Add(_emptyStringNullableDecimal);
            _mainContainer.Children.Add(_allowZeroNullableDecimal);

            this.Content = _mainContainer;
        }

        protected override void BindControls()
        {
            this.Bind(ViewModel, x => x.IntProperty, c => c._allowZeroInt.Text,
                vmToViewConverterOverride: new TypeConverters.NumericToStringConverter(false),
                viewToVMConverterOverride: new TypeConverters.ToNumericConverter())
                .DisposeWith(ControlBindings);

            this.Bind(ViewModel, x => x.SecondIntProperty, c => c._emptyStringInt.Text,
                vmToViewConverterOverride: new TypeConverters.NumericToStringConverter(),
                viewToVMConverterOverride: new TypeConverters.ToNumericConverter())
                .DisposeWith(ControlBindings);

            this.Bind(ViewModel, x => x.DecimalProperty, c => c._allowZeroDecimal.Text,
                vmToViewConverterOverride: new TypeConverters.NumericToStringConverter(false),
                viewToVMConverterOverride: new TypeConverters.ToNumericConverter())
                .DisposeWith(ControlBindings);

            this.Bind(ViewModel, x => x.SecondDecimalProperty, c => c._emptyStringDecimal.Text,
                vmToViewConverterOverride: new TypeConverters.NumericToStringConverter(),
                viewToVMConverterOverride: new TypeConverters.ToNumericConverter())
                .DisposeWith(ControlBindings);

            this.Bind(ViewModel, x => x.NullableDecimalProperty, c => c._allowZeroNullableDecimal.Text,
                vmToViewConverterOverride: new TypeConverters.NumericToStringConverter(false),
                viewToVMConverterOverride: new TypeConverters.ToNumericConverter())
                .DisposeWith(ControlBindings);

            this.Bind(ViewModel, x => x.SecondNullableDecimalProperty, c => c._emptyStringNullableDecimal.Text,
                vmToViewConverterOverride: new TypeConverters.NumericToStringConverter(),
                viewToVMConverterOverride: new TypeConverters.ToNumericConverter())
                .DisposeWith(ControlBindings);
        }
    }
}
