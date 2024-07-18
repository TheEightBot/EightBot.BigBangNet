using System;
using Microsoft.Maui;

namespace EightBot.BigBang.Maui
{
    public static class ResourceDictionaryExtensions
    {
        public static void Replace(this ResourceDictionary dictionary, Style implicitStyle)
        {
            var key = implicitStyle.TargetType.FullName;

            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);

            dictionary.Add(implicitStyle);
        }
    }
}

