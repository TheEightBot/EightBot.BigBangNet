using System;
using FluentValidation;

namespace EightBot.BigBang.Sample.Validators
{
	public class SampleViewModelValidator : AbstractValidator<ViewModels.SampleViewModel>
	{
		public SampleViewModelValidator()
		{
		}
	}
}
