using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Qbank.Core;
using Qbank.Core.Event;
using Qbank.Core.Orchestrations;
using Qbank.Core.Orchestrations.Impl;
using Qbank.Test.Core.Orchestrations;

namespace Qbank.Test
{
    /// <summary>
    /// Base class for orchestrations' tests
    /// </summary>
    /// <typeparam name="TOrchestration"></typeparam>
    /// <typeparam name="TOrchestrationInput"></typeparam>
    public abstract class OrchestrationBaseTest<TOrchestration, TOrchestrationInput>
        where TOrchestration : Orchestration<TOrchestrationInput>
    {
        TOrchestrationInput input;
        DateTimeOffset date;
        readonly Queue<TimeSpan> deltas = new Queue<TimeSpan>();
        readonly Queue<Guid> guids = new Queue<Guid>();
        readonly List<IEvent> events = new List<IEvent>();

        [SetUp]
        public void SetUp()
        {
            input = default(TOrchestrationInput);
            date = DateTimeOffset.MinValue;
            deltas.Clear();
            guids.Clear();
            events.Clear();
        }

        [TearDown]
        public async Task TearDown()
        {
            var orchestration = Build();
            var context = new OrchestrationContext(this);

            await Task.WhenAny(
                ((IOrchestration)orchestration).Execute(orchestration, context, Enumerable.Empty<IEvent>(),
                    new CallSerializer()),
                context.Task);
        }

        protected abstract TOrchestration Build();

        protected void Given(TOrchestrationInput orchestrationInput)
        {
            input = orchestrationInput;
        }

        protected void StartAt(DateTimeOffset date)
        {
            this.date = date;
        }

        protected void NextDateAfter(TimeSpan span)
        {
            deltas.Enqueue(span);
        }

        protected void NextGuid(Guid guid)
        {
            guids.Enqueue(guid);
        }

        class OrchestrationContext : IOrchestrationContext
        {
            readonly OrchestrationBaseTest<TOrchestration, TOrchestrationInput> _test;
            readonly TaskCompletionSource<object> _completionSource;

            public OrchestrationContext(OrchestrationBaseTest<TOrchestration, TOrchestrationInput> test)
            {
                this._test = test;
                _completionSource = new TaskCompletionSource<object>();
            }

            public Task Task => _completionSource.Task;

            public Guid NewGuid()
            {
                if (_test.guids.Count == 0)
                {
                    throw new InvalidOperationException("No more Guid values were registered");
                }

                return _test.guids.Dequeue();
            }

            public DateTimeOffset DateTimeUtcNow()
            {
                if (_test.deltas.Count == 0)
                {
                    throw new InvalidOperationException("No more TimeSpan values were registered");
                }

                return _test.date + _test.deltas.Dequeue();
            }

            public void EndCurrentExecution()
            {
                _completionSource.SetResult(this);
            }

            public void Append(IEvent @event)
            {
                _test.events.Add(@event);
            }

            public Task Flush() => Task.CompletedTask;
        }
    }
}