using System;
using System.Reactive;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using EightBot.BigBang.ViewModel;
using FluentValidation;
using ReactiveUI;
using System.Reactive.Disposables;

namespace EightBot.BigBang.Sample.ViewModels
{
    [PreCache]
    public class SampleViewModel : ValidationViewModelBase<SampleViewModel>
    {
        public SampleViewModel()
        {
        }

        public override string Title => "Sample";

        Lazy<Validators.SampleViewModelValidator> _validator =
            new Lazy<Validators.SampleViewModelValidator>(() => new Validators.SampleViewModelValidator());

        public override AbstractValidator<SampleViewModel> Validator => _validator.Value;

        int _intProperty;

        [DataMember]
        public int IntProperty
        {
            get { return _intProperty; }
            set { this.RaiseAndSetIfChanged(ref _intProperty, value); }
        }

        int _secondIntProperty;

        [DataMember]
        public int SecondIntProperty
        {
            get { return _secondIntProperty; }
            set { this.RaiseAndSetIfChanged(ref _secondIntProperty, value); }
        }

        string _stringProperty;

        [DataMember]
        public string StringProperty
        {
            get { return _stringProperty; }
            set { this.RaiseAndSetIfChanged(ref _stringProperty, value); }
        }

        bool _boolProperty;

        [DataMember]
        public bool BoolProperty
        {
            get { return _boolProperty; }
            set { this.RaiseAndSetIfChanged(ref _boolProperty, value); }
        }

        decimal _decimalProperty;

        [DataMember]
        public decimal DecimalProperty
        {
            get { return _decimalProperty; }
            set { this.RaiseAndSetIfChanged(ref _decimalProperty, value); }
        }

        decimal _secondDecimalProperty;

        [DataMember]
        public decimal SecondDecimalProperty
        {
            get { return _secondDecimalProperty; }
            set { this.RaiseAndSetIfChanged(ref _secondDecimalProperty, value); }
        }

        decimal? _nullableDecimalProperty;

        [DataMember]
        public decimal? NullableDecimalProperty
        {
            get { return _nullableDecimalProperty; }
            set { this.RaiseAndSetIfChanged(ref _nullableDecimalProperty, value); }
        }

        decimal? _secondNullableDecimalProperty;

        [DataMember]
        public decimal? SecondNullableDecimalProperty
        {
            get { return _secondNullableDecimalProperty; }
            set { this.RaiseAndSetIfChanged(ref _secondNullableDecimalProperty, value); }
        }

        Models.SampleModel _data;

        [DataMember]
        public Models.SampleModel Data
        {
            get { return _data; }
            set { this.RaiseAndSetIfChanged(ref _data, value); }
        }

        ReactiveCommand<Unit, Unit> _unitCommand;

        [DataMember]
        public ReactiveCommand<Unit, Unit> UnitCommand
        {
            get { return _unitCommand; }
            set { this.RaiseAndSetIfChanged(ref _unitCommand, value); }
        }

        protected override void RegisterObservables()
        {
            UnitCommand = ReactiveCommand
                .CreateFromTask(async _ =>
                {
                    var rngesus = new Random(Guid.NewGuid().GetHashCode());
                    System.Diagnostics.Debug.WriteLine($"Unit Command activated for {StringProperty}");
                    await Task.Delay(rngesus.Next(0, 300));
                })
                .DisposeWith(ViewModelBindings);
        }
    }
}
