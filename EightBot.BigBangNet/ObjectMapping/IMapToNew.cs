using System;

namespace EightBot.BigBang.ObjectMapping
{
    public interface IMapToNew<TSource, TTarget>
    {
        TTarget Map(TSource source);
    }
}

