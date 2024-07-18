using System;
using System.Text.RegularExpressions;
using ReactiveUI;

namespace EightBot.BigBang.TypeConverters
{
    public class ToCurrencyConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (toType == typeof(double) ? 1 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            var nullableType = Nullable.GetUnderlyingType(toType);
            var isNullableType = nullableType != null;

            if (isNullableType)
                toType = nullableType;

            try
            {
                var str = from.ToString();

                if (string.IsNullOrEmpty(str))
                {
                    result = isNullableType ? null : Convert.ChangeType(0, toType);
                    return true;
                }

                double retNum;
                bool isNum = Double.TryParse(str, System.Globalization.NumberStyles.Currency, System.Globalization.NumberFormatInfo.CurrentInfo, out retNum);

                retNum = Math.Round(retNum, 2);

                if (isNum)
                {
                    result = Convert.ChangeType(retNum, toType);

                    return true;
                }

                result = isNullableType ? null : Convert.ChangeType(0, toType);

                return true;
            }
            catch (Exception)
            {
                result = isNullableType ? null : Convert.ChangeType(0, toType);
                return false;
            }
        }
    }

    public class NumericToCurrencyStringConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (toType == typeof(string) ? 1 : 0);
        }


        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                if (from == null)
                {
                    result = string.Empty;
                    return true;
                }

                var fromString = from.ToString();

                var val = double.Parse(fromString, System.Globalization.NumberStyles.Currency);

                var decimalSeparator = System.Globalization.NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;

				var valStr = val.ToString();

				if (valStr.EndsWith(decimalSeparator, StringComparison.OrdinalIgnoreCase))
				{
					result = valStr.Substring(0, valStr.Length - decimalSeparator.Length);
					return true;
				}

                var decimalPlaces = 
                    valStr.Contains(decimalSeparator)
                        ? valStr.Substring(valStr.IndexOf(decimalSeparator) + 1).Length
                        : -1;

                var formattedValue = val.ToString("C");


                if (decimalPlaces == 1)
                    result = formattedValue.Remove(formattedValue.Length - 1, 1);
                else if (decimalPlaces == 0)
                    result = formattedValue.Remove(formattedValue.Length - 2, 2);
				else if (decimalPlaces == -1)
                    result = formattedValue.Remove(formattedValue.Length - 3, 3);
                else 
                    result = formattedValue;

                return true;
            }
            catch (Exception)
            {
                result = 0d.ToString("C");
                return false;
            }
        }
    }
}
