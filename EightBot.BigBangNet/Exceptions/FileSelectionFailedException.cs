using System;
using System.Runtime.Serialization;

namespace EightBot.BigBang.Exceptions
{
    public class FileSelectionFailedException : Exception
    {
        public FileSelectionFailedException()
        {
        }

        public FileSelectionFailedException(string message) : base(message)
        {
        }

        public FileSelectionFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FileSelectionFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
