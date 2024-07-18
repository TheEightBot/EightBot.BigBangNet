using System;
using ReactiveUI;

namespace EightBot.BigBang.TypeConverters
{
    public class DateTimeToDateTimeOffsetConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(DateTime) && toType == typeof(DateTimeOffset) ? 1 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                var val = (DateTime)from;

                result = new DateTimeOffset(val);
                return true;
            }
            catch (Exception)
            {
                result = Activator.CreateInstance(toType);
                return false;
            }
        }
    }

    public class DateTimeOffsetToDateTimeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(DateTimeOffset) && toType == typeof(DateTime) ? 1 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                var val = (DateTimeOffset)from;

                result = val.LocalDateTime;
                return true;
            }
            catch (Exception)
            {
                result = Activator.CreateInstance(toType);
                return false;
            }
        }
    }

    public class NullableDateTimeToNullableDateTimeOffsetConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(DateTime?) && toType == typeof(DateTimeOffset?) ? 1 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                var val = (DateTime?)from;

                result = val.HasValue ? new DateTimeOffset?(val.Value) : new DateTimeOffset?();
                return true;
            }
            catch (Exception)
            {
                result = Activator.CreateInstance(toType);
                return false;
            }
        }
    }

    public class NullableDateTimeOffsetToNullableDateTimeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(DateTimeOffset?) && toType == typeof(DateTime?) ? 1 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            try
            {
                var val = (DateTimeOffset?)from;

                result = val.HasValue ? val.Value.LocalDateTime : new DateTime?();
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

