using System;
using System.Runtime.Serialization;
using EightBot.BigBang;
using EightBot.BigBang.ViewModel;
using FluentValidation;
using ReactiveUI;

namespace EightBot.BigBang.Sample.ViewModels
{
    public class NullableDatePickerViewModel : ViewModelBase
    {
        public override string Title => "Nullable Date Picker View Model";

        DateTime? _nullableDateTime;

        [DataMember]
        public DateTime? NullableDateTime
        {
            get { return _nullableDateTime; }
            set { this.RaiseAndSetIfChanged(ref _nullableDateTime, value); }
        }

        DateTimeOffset? _NullableDateTimeOffset;

        [DataMember]
        public DateTimeOffset? NullableDateTimeOffset
        {
            get { return _NullableDateTimeOffset; }
            set { this.RaiseAndSetIfChanged(ref _NullableDateTimeOffset, value); }
        }

        DateTime? _nullablDateTimeOffset2;

        [DataMember]
        public DateTime? NullableDateTimeOffset2 {
            get { return _nullablDateTimeOffset2; }
            set { this.RaiseAndSetIfChanged(ref _nullablDateTimeOffset2, value); }
        }

        protected override void RegisterObservables()
        {
        }
    }
}

