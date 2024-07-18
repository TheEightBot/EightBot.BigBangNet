using System;

namespace EightBot.BigBang
{
	public static class StringExtensions
	{
		public delegate bool TryDelegate<T>(string s, out T result);

		public static bool IsNumeric(this string stringToValidate){
			double parsed;

			return Double.TryParse (
				stringToValidate, 
				System.Globalization.NumberStyles.Any,
				System.Globalization.NumberFormatInfo.InvariantInfo, 
				out parsed);
		}

		public static bool TryParseNullable<T>(this string s, out T? result, TryDelegate<T> tryDelegate) where T : struct
		{
			if (s == null)
			{
				result = null;
				return true;
			}

			T temp;
			bool success = tryDelegate(s, out temp);
			result = temp;
			return success;
		}

		public static T? ParseNullable<T>(this string s, TryDelegate<T> tryDelegate) where T : struct
		{
			if (s == null)
			{
				return null;
			}

			T temp;
			return tryDelegate(s, out temp)
				? (T?)temp
					: null;
		} 

		public static bool Contains(this string s, string innerString, StringComparison comparisonType){
			return s.IndexOf (innerString, comparisonType) >= 0;
		}

		public static bool IsNullOrEmpty (this string s) {
			return string.IsNullOrEmpty (s);
		}

		public static bool IsNotNullOrEmpty (this string s)
		{
			return !string.IsNullOrEmpty (s);
		}

		public static bool IsNullOrWhiteSpace (this string s)
		{
			return string.IsNullOrWhiteSpace (s);
		}

		public static bool IsNotNullOrWhiteSpace (this string s)
		{
			return !string.IsNullOrWhiteSpace (s);
		}
	}
}

