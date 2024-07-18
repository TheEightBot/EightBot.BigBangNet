using System;
using Splat;
using ReactiveUI;

namespace EightBot.BigBang
{
	public static class BootStrap
	{
		public static void Init()
		{
			#if DEBUG
			var debugLogger = new DebugLogger(){ Level = LogLevel.Debug };
			Locator.CurrentMutable.RegisterConstant(debugLogger, typeof(ILogger));
			#endif
		}
	}
}

