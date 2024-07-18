using FluentValidation;

namespace EightBot.BigBang
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> Numeric<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(BeNumeric).WithMessage("'{PropertyName}' must be a number.");
        }

        static bool BeNumeric(string stringIn)
        {
            return stringIn.IsNumeric();
        }
    }
}

