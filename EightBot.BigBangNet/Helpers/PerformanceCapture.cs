using System;
using System.Diagnostics;

namespace EightBot.BigBang.Helpers
{
	/// <remarks>
	/// Snagged from this post on StackOverflow
	/// https://stackoverflow.com/questions/13681664/helper-class-for-peformance-tests-using-stopwatch-class
	/// </remarks>
	public class PerformanceCapture : IDisposable
	{
		Stopwatch _stopwatch = new Stopwatch();
		Action<TimeSpan> _callback;

		public PerformanceCapture()
		{
			_stopwatch.Start();
		}

		public PerformanceCapture(Action<TimeSpan> callback) : this()
		{
			_callback = callback;            
		}

		public static PerformanceCapture Start(Action<TimeSpan> callback)
		{
			return new PerformanceCapture(callback);
		}

		public void Dispose()
		{
			_stopwatch.Stop();
			_callback?.Invoke(Result);
		}

		public TimeSpan Result
		{
			get { return _stopwatch.Elapsed; }
		}
	}
}

