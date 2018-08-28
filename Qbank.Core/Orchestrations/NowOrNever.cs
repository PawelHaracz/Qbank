using System;
using System.Runtime.CompilerServices;

namespace Qbank.Core.Orchestrations
{
    public sealed class NowOrNever : ICriticalNotifyCompletion
    {
        public static readonly NowOrNever Never = new NowOrNever(false);
        public static readonly NowOrNever Now = new NowOrNever(true);

        NowOrNever(bool isCompleted)
        {
            IsCompleted = isCompleted;
        }

        public NowOrNever GetAwaiter()
        {
            return this;
        }

        public void GetResult() { }

        public bool IsCompleted { get; }

        public void OnCompleted(Action continuation) { }

        public void UnsafeOnCompleted(Action continuation) { }
    }
}