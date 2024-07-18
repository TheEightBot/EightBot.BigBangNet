using System;
using System.Linq;
using System.Collections.Generic;

namespace EightBot.BigBang
{
    public static class EnumExtensions
    {
        public static List<ViewModel.KeyValueContainer<string, int>> ConvertToListKeyValueViewModel<TEnum>()
        {

            List<ViewModel.KeyValueContainer<string, int>> enumValues = new List<ViewModel.KeyValueContainer<string, int>>();

            foreach (TEnum item in Enum.GetValues(typeof(TEnum)))
                enumValues.Add(
                    new ViewModel.KeyValueContainer<string, int>
                    {
                        Key = item.ToString(),
                        Value = Convert.ToInt32(item)
                    });

            return enumValues;
        }
    }
}

