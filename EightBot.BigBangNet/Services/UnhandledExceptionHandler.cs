using System;
using System.Linq;

namespace EightBot.BigBang.Services
{
    public abstract class UnhandledExceptionHandler
    {
        public Type[] HandledExceptions
        {
            get;
            private set;
        }

        protected UnhandledExceptionHandler(Type[] handledExceptions)
        {

            if (handledExceptions == null || !handledExceptions.Any())
                throw new ArgumentException("you must provided exceptions to handle");

            var nonExceptions = handledExceptions.Where(t => t != typeof(Exception)).ToList();

            if (nonExceptions.Any())
                throw new ArgumentException(string.Format("The following types are not exceptions: {0}", string.Join(",", nonExceptions)));

            HandledExceptions = handledExceptions;
        }

        public abstract void StartListening();

        public abstract void StopListening();
    }
}

