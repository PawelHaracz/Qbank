using System;

namespace Qbank.Core.Orchestrations
{
    public class TaskFailedException : Exception
    {
        public TaskFailedException(string message, string stackTrace)
            : base(message)
        {
            StackTrace = stackTrace;
        }

        public override string StackTrace { get; }
    }
}