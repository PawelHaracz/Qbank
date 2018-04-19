using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Orchestrations.Impl;

namespace Qbank.Test.Core.Orchestrations
{
    class TestOrchestrationContext : IOrchestrationContext
    {
        static readonly DateTimeOffset Date0 = new DateTimeOffset(1000, 1, 1, 1, 1, 1, TimeSpan.Zero);

        public static readonly Guid[] Guids =
        {
            new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1),
            new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2),
            new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3),
            new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4),
            new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 5)
        };

        public static readonly DateTimeOffset[] Dates =
        {
            Date0.AddSeconds(0),
            Date0.AddSeconds(1),
            Date0.AddSeconds(2),
            Date0.AddSeconds(3),
            Date0.AddSeconds(4),
        };

        readonly Queue<Guid> guids = new Queue<Guid>(Guids);
        readonly Queue<DateTimeOffset> dates = new Queue<DateTimeOffset>(Dates);
        readonly TaskCompletionSource<object> end = new TaskCompletionSource<object>();
        readonly List<IEvent> events = new List<IEvent>();

        public Guid NewGuid() => guids.Dequeue();
        public DateTimeOffset DateTimeUtcNow() => dates.Dequeue();

        public Task Flush()
        {
            return Task.CompletedTask;
        }

        public IEnumerable<IEvent> Events => events;

        public void EndCurrentExecution()
        {
            end.SetResult(new object());
        }

        public Task EndTask => end.Task;

        public void Append(IEvent @event)
        {
            events.Add(@event);
        }
    }
}