using System;
using ReactiveUI;

namespace EightBot.BigBang.TypeConverters
{
    public class NullableTimeSpanToNullableTimeSpanConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(TimeSpan?) && toType == typeof(TimeSpan?) ? 1 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                var val = (TimeSpan?)from;

                result = val.HasValue ? new TimeSpan?(val.Value) : new TimeSpan?();
                return true;
            }
            catch (Exception)
            {
                result = Activator.CreateInstance(toType);
                return false;
            }
        }
    }
}

