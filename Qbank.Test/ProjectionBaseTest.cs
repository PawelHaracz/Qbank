using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using ProtoBuf;
using Qbank.Core;
using Qbank.Core.Event;
using Qbank.Core.Projections;

namespace Qbank.Test
{
    /// <summary>
    /// Base class for projection tests.
    /// </summary>
    /// <typeparam name="TProjection"></typeparam>
    /// <typeparam name="TState"></typeparam>
    public abstract class ProjectionBaseTest<TProjection, TState>
        where TProjection : ProjectionBase<TProjection, TState>, new() where TState : new()
    {
        readonly Dictionary<string, TState> then = new Dictionary<string, TState>();
        readonly List<Tuple<string, IEvent>> given = new List<Tuple<string, IEvent>>();

        [SetUp]
        public void SetUp()
        {
            given.Clear();
            then.Clear();
        }

        /// <summary>
        /// Registers a set of events that is applied to the aggregate before the call.
        /// </summary>
        /// <param name="events">Events to be applied.</param>
        protected void Given(params IEvent[] events)
        {
            Given(Guid.NewGuid().ToString(), events);
        }

        /// <summary>
        /// Registers a set of events that is applied to the aggregate before the call.
        /// </summary>
        /// <param name="streamId">Stream id.</param>
        /// <param name="events">Events to be applied.</param>
        protected void Given(string streamId, params IEvent[] events)
        {
            given.AddRange(events.Select(e => Tuple.Create(streamId, e)));
        }

        protected void Then(string name, TState state)
        {
            then.Add(name, state);
        }

        [TearDown]
        public async Task TearDown()
        {
            var result = new Dictionary<string, TState>();
            var p = new TProjection();
            foreach (var @event in given)
            {
                var e = @event.Item2;

                var type = e.GetType();
                var attr = type.GetCustomAttribute<EventTypeIdAttribute>();
                if (attr == null)
                    throw new ArgumentException($"Mark e '{type.FullName}' with {nameof(EventTypeIdAttribute)}");
                var id = attr.Id;

                var dispatchedEvent = new DispatchedEvent
                {
                    Event = e,
                    Metadata = new Metadata
                    {
                        EventId = Guid.NewGuid(),
                        EventTypeId = id,
                        StreamId = @event.Item1
                    }
                };

                var partition = p.GetPartitioningKey(dispatchedEvent);
                TState state;
                if (result.TryGetValue(partition, out state) == false)
                {
                    result[partition] = state = new TState();
                }

                await p.Dispatch(dispatchedEvent, state).ConfigureAwait(false);
            }

            var expected = Serialize(then);
            var observed = Serialize(result);

            var areEqual = expected.SequenceEqual(observed);
            if (areEqual == false)
            {
                Console.WriteLine("GIVEN:");
                using (ConsoleExtensions.Indent())
                {
                    Write(given);
                }

                Console.WriteLine("THEN should be:");
                using (ConsoleExtensions.Indent())
                {
                    Write(then);
                }

                Console.WriteLine("BUT was:");
                using (ConsoleExtensions.Indent())
                {
                    Write(result);
                }

                TestExecutionContext.CurrentContext.CurrentResult.
                    SetResult(ResultState.ChildFailure, "The state was different from expected.", " ");
            }
        }

        static void Write(IEnumerable<Tuple<string, IEvent>> events)
        {
            foreach (var @event in events)
                Write(@event);
        }

        static void Write(Dictionary<string, TState> then)
        {
            foreach (var kvp in then)
            {
                Console.WriteLine($"- [{kvp.Key}] = '{kvp.Value}'");
            }
        }

        static void Write(Tuple<string, IEvent> e)
        {
            Console.WriteLine($"- {e.Item2} for {e.Item1}");
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        static byte[] Serialize(Dictionary<string, TState> states)
        {
            var kvs = states.OrderBy(s => s.Key).ToArray();

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, kvs);
                return ms.ToArray();
            }
        }
    }
}