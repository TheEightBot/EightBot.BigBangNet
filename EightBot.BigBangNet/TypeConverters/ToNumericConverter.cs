using System;
using ReactiveUI;

namespace EightBot.BigBang.TypeConverters
{
	public class ToNumericConverter : IBindingTypeConverter
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
				bool isNum = Double.TryParse(str, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);

				if (isNum)
				{
                    result = Convert.ChangeType(retNum, toType);

					return true;
				}

				result = isNullableType ? null : Convert.ChangeType(0, toType);

				return true;
			}
			catch (Exception) {
				result = isNullableType ? null : Convert.ChangeType(0, toType);
				return false;
			}
		}
	}

	public class NumericToStringConverter : IBindingTypeConverter
	{
		bool _zeroIsEmptyString;

		public NumericToStringConverter(bool zeroIsEmptyString = true)
		{
			_zeroIsEmptyString = zeroIsEmptyString;
		}

		public int GetAffinityForObjects(Type fromType, Type toType)
		{
			return (toType == typeof(string) ? 1 : 0);
		}

		public bool TryConvert(object from, Type toType, object conversionHint, out object result)
		{
            try {
                if (from == null) {
                    result = string.Empty;
                    return true;
                }
                
                var fromString = from.ToString();
                
                var val = double.Parse(fromString, System.Globalization.NumberStyles.Any);
    
				if (Math.Abs(val) < .001d && _zeroIsEmptyString)
                {
                    result = string.Empty;
                    return true;
                }
    
                result = val.ToString();
    
                return true;
            } catch (Exception) {
                result = string.Empty;
                return false;
            }
		}
	}
}

