using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Qbank.Core;
using Qbank.Core.Orchestrations;
using Qbank.Core.Orchestrations.Impl;

namespace Qbank.Test.Core.Orchestrations
{
    public class OrchestrationTests
    {
        CallSerializer serializer;
        IEvent[] when;
        Func<IOrchestration> orchestrationFactory;
        const int InputValue = 1;

        [SetUp]
        public void SetUp()
        {
            serializer = new CallSerializer();
            orchestrationFactory = null;
            when = null;
        }

        [Test]
        public async Task SimpleGuid()
        {
            For<GuidOrchestration>();

            await Then(
                new GuidGenerated(TestOrchestrationContext.Guids[0]),
                OrchestrationEnded.Instance);
        }

        class GuidOrchestration : Orchestration<int>
        {
            protected override Task Execute(int input)
            {
                var guid = NewGuid();
                return Task.CompletedTask; ;
            }
        }

        [Test]
        public async Task SimpleDate()
        {
            For<DateTimeOrchestration>();

            await Then(new DateTimeRetrieved(TestOrchestrationContext.Dates[0]),
                OrchestrationEnded.Instance);
        }

        class DateTimeOrchestration : Orchestration<int>
        {
            protected override Task Execute(int input)
            {
                var date = GetDateTimeUtcNow();
                return Task.CompletedTask;
            }
        }

        [Test]
        public async Task DelayInTheFuture()
        {
            var delay = TimeSpan.FromDays(1);
            var date = TestOrchestrationContext.Dates[0];

            For(() => new DelayOrchestration(delay));

            await Then(
                new DateTimeRetrieved(date),
                new ScheduledAt(date + delay));
        }

        [Test]
        public async Task DelayInThePast()
        {
            var delay = TimeSpan.FromDays(-1);
            var date = TestOrchestrationContext.Dates[0];

            For(() => new DelayOrchestration(delay));

            await Then(
                new DateTimeRetrieved(date),
                OrchestrationEnded.Instance);
        }

        [Test]
        public async Task DelayInThePastReplayed()
        {
            var delay = TimeSpan.FromDays(-1);
            var date = TestOrchestrationContext.Dates[0];

            For(() => new DelayOrchestration(delay));
            When(new DateTimeRetrieved(date), new ScheduledAt(date + delay));
            await Then(OrchestrationEnded.Instance);
        }

        [Test]
        public async Task DelayInTheFutureReplayed()
        {
            var delay = TestOrchestrationContext.Dates[2] - TestOrchestrationContext.Dates[0];
            var date = TestOrchestrationContext.Dates[0];

            For(() => new DelayOrchestration(delay));
            When(new DateTimeRetrieved(date), new ScheduledAt(date + delay));
            await Then();
        }

        class DelayOrchestration : Orchestration<int>
        {
            readonly TimeSpan delay;

            public DelayOrchestration(TimeSpan delay)
            {
                this.delay = delay;
            }

            protected override async Task Execute(int input)
            {
                await Delay(delay);
            }
        }

        [Test]
        public async Task CallingServices()
        {
            var g1 = TestOrchestrationContext.Guids[0];
            var g2 = TestOrchestrationContext.Guids[1];
            var g3 = TestOrchestrationContext.Guids[2];

            const int r1 = CallingOrchestration.CallResult1;
            const int r2 = CallingOrchestration.CallResult2;
            const int r3 = CallingOrchestration.CallResult3;

            var calls = new Dictionary<Guid, int>
            {
                {g1, r1},
                {g2, r2},
                {g3, r3}
            };

            For(() => new CallingOrchestration(calls));

            await Then(
                new GuidGenerated(g1),
                new CallRecorded(Serialize(r1)),
                new GuidGenerated(g2),
                new CallRecorded(Serialize(r2)),
                new GuidGenerated(g3),
                new CallRecorded(Serialize(r3)),
                OrchestrationEnded.Instance);
        }

        class CallingOrchestration : Orchestration<int>
        {
            public const int CallResult1 = 1;
            public const int CallResult2 = 2;
            public const int CallResult3 = 3;

            readonly Dictionary<Guid, int> calls;

            public CallingOrchestration(Dictionary<Guid, int> calls)
            {
                this.calls = calls;
            }

            protected override async Task Execute(int input)
            {
                var result1 = await Call(DoCall);
                Assert.AreEqual(CallResult1, result1);
                var result2 = await Call(DoCall);
                Assert.AreEqual(CallResult2, result2);
                var result3 = await Call(DoCall);
                Assert.AreEqual(CallResult3, result3);
            }

            Task<int> DoCall(Guid callid)
            {
                return Task.FromResult(calls[callid]);
            }
        }

        [Test]
        public async Task CallingWithException()
        {
            var callId = TestOrchestrationContext.Guids[0];

            For(() => new FailingCallOrchestration());

            await Then(
                new GuidGenerated(callId),
                new CallRecorded(FailingCallOrchestration.ExceptionMessage, FailingCallOrchestration.ExceptionStackTrace));
        }

        class FailingCallOrchestration : Orchestration<int>
        {
            public const string ExceptionMessage = "This is test exception message";
            public const string ExceptionStackTrace = "This is test stack trace";

            protected override async Task Execute(int input)
            {
                await Call<int>(async callId =>
                {
                    await Task.CompletedTask;
                    throw new CustomException(ExceptionMessage, ExceptionStackTrace);
                });
            }

            class CustomException : Exception
            {
                public CustomException(string message, string stackTrace) : base(message)
                {
                    StackTrace = stackTrace;
                }

                public override string StackTrace { get; }
            }
        }

        byte[] Serialize<T>(T callResult)
        {
            return serializer.Serialize(callResult);
        }

        void For<TOrchestration>()
            where TOrchestration : IOrchestration, new()
        {
            For(() => new TOrchestration());
        }

        void For(Func<IOrchestration> factory)
        {
            this.orchestrationFactory = factory;
        }

        void When(params IEvent[] inputEvents)
        {
            when = inputEvents;
        }

        async Task Then(params IEvent[] expectedEvents)
        {
            var inputEvents = (when ?? new IEvent[0]).ToArray();
            var allEvents = inputEvents.Concat(expectedEvents).ToArray();

            // iteration from any point of history ensures that whenever orchestration is lost, it will be able to pick up its execution
            for (var i = inputEvents.Length; i <= allEvents.Length; i++)
            {
                var context = new TestOrchestrationContext();

                var initialEvents = allEvents.Take(i).ToArray();

                BurnUnneededValues(initialEvents, context);

                var orchestration = orchestrationFactory();
                var execution = orchestration.Execute(InputValue, context, initialEvents, serializer);

                var ended = await Task.WhenAny(execution, context.EndTask);
                await orchestration.FlushEvents();

                var endedBeforeEndOfOrchestration = ended == context.EndTask;

                CollectionAssert.AreEqual(allEvents.Skip(i).ToArray(), context.Events.ToArray(), new EventComparer(), $"Failed when comparing emitted events at '{i}' run");
            }
        }

        static void BurnUnneededValues(IEnumerable<IEvent> initialEvents, IOrchestrationContext context)
        {
            foreach (var @event in initialEvents)
            {
                if (@event is GuidGenerated)
                    context.NewGuid();
                if (@event is DateTimeRetrieved)
                    context.DateTimeUtcNow();
            }
        }
    }
}