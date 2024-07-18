using System;
using System.Threading.Tasks;

namespace EightBot.BigBang
{
	public static class TaskExtensions
	{
        /// <summary>
        /// Use this to suppress the compiler warning when firing off a task and not awaiting.
        /// </summary>
        /// <remarks>
        /// Suppresses warning CS4014: "Because this call is not awaited, execution of the current method continues"
        /// Also, you should REALLY AVOID using this, in 99.9% of cases it is a "bad move"™
        /// </remarks>
		public static void FireAndForget(this Task task)
		{
		}
	}
}

