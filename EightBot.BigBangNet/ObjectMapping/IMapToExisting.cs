using System;

namespace EightBot.BigBang.ObjectMapping
{
	public interface IMapToExisting<TSource, TTarget>
	{
		void Map(TSource source, TTarget target);
	}
}

