using System;

namespace EightBot.BigBang.ObjectMapping
{
    public static class MappingExtensions
    {
        public static void MapTo<TSource, TTarget>(this TTarget target, TSource source, IMapToExisting<TSource, TTarget> mapper)
        {
            mapper.Map(source, target);
        }

        public static TTarget MapTo<TSource, TTarget>(this TSource source, IMapToNew<TSource, TTarget> mapper)
        {
            return mapper.Map(source);
        }
    }
}

