using System;
using System.Runtime.Serialization;
using EightBot.BigBang;
using EightBot.BigBang.ViewModel;
using FluentValidation;
using ReactiveUI;

namespace EightBot.BigBang.Sample.ViewModels
{
    public class NullableTimePickerViewModel : ViewModelBase
    {
        public override string Title => "Nullable Time Picker View Model";

        TimeSpan? _nullableTimeSpan;

        [DataMember]
        public TimeSpan? NullableTimeSpan
        {
            get { return _nullableTimeSpan; }
            set { this.RaiseAndSetIfChanged(ref _nullableTimeSpan, value); }
        }

        protected override void RegisterObservables()
        {
        }
    }
}

