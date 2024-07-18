using System;
using Xamarin.Forms;

namespace EightBot.BigBang.XamForms
{
	public static class ResourceDictionaryExtensions
	{
		public static void Replace(this ResourceDictionary dictionary, Style implicitStyle){
			var key = implicitStyle.TargetType.FullName;

			if (dictionary.ContainsKey (key))
				dictionary.Remove (key);

			dictionary.Add (implicitStyle);
		}
	}
}

