using System;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace EightBot.BigBang.ViewModel
{
	public class ValidationInformation : ReactiveObject
	{
		[Reactive]
        public object AttemptedValue { get; set; }

		[Reactive]
		public string ErrorCode { get; set; }

		[Reactive]
		public string ErrorMessage { get; set; }

		[Reactive]
		public string PropertyName { get; set; }

		[Reactive]
		public bool IsError { get; set; }
		public ValidationInformation(string propertyName, bool isValid = true)
		{
			this.PropertyName = propertyName;
			this.IsError = isValid;
		}

		public ValidationInformation(string propertyName, string error) : this (propertyName, error, null)
		{
		}

		public ValidationInformation(string propertyName, string error, object attemptedValue)
		{
			this.PropertyName = propertyName;
			this.ErrorMessage = error;
			this.AttemptedValue = attemptedValue;
			this.IsError = false;
		}
	}
}
