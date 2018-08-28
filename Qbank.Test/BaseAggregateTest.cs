using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using ProtoBuf;
using Qbank.Core;
using Qbank.Core.Event;

namespace Qbank.Test
{
    /// <summary>
    ///     The base aggregate test class that enables BDD-like <see cref="Given" />,
    ///     <see cref="Then(Action)" />.
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    public abstract class BaseAggregateTest<TState>
        where TState : BaseState, new()
    {
        readonly List<IEvent> given = new List<IEvent>();
        readonly List<IEvent> when = new List<IEvent>();
        readonly List<IEvent> then = new List<IEvent>();
        string action;
        Action<TState> assert;
        bool expectFailure;

        [SetUp]
        public void SetUp()
        {
            given.Clear();
            when.Clear();
            then.Clear();
            assert = a => { };
            expectFailure = false;
        }

        /// <summary>
        ///     Registers a set of events that is applied to the aggregate before the call.
        /// </summary>
        /// <param name="events">Events to be applied.</param>
        protected void Given(params IEvent[] events)
        {
            given.AddRange(events);
        }

        /// <summary>
        ///     Executes the command on an aggregate.
        /// </summary>
        /// <param name="action">The action to be exectured.</param>
        protected void When(Expression<Func<TState, IEnumerable<IEvent>>> action)
        {
            this.action = DiscoverName(action);
            var state = ReplyAll();

            var events = action.Compile()(state).ToArray();

            when.AddRange(events);
        }

        TState ReplyAll()
        {
            var state = new TState();

            foreach (var @event in given)
                StateApplier.Apply(state, @event);
            foreach (var @event in when)
                StateApplier.Apply(state, @event);
            return state;
        }

        /// <summary>
        ///     Asserts the events emitted due to handling the command
        /// </summary>
        protected void Then(params IEvent[] events)
        {
            then.AddRange(events);
        }

        /// <summary>
        ///     Asserts the aggregate state after applying emitted events.
        /// </summary>
        protected void Then(Action<TState> assertAggregate)
        {
            assert += assertAggregate;
        }

        protected void ExpectFailure()
        {
            expectFailure = true;
        }

        [TearDown]
        public void TearDown()
        {
            var result = when.ToArray();
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

                Console.WriteLine("WHEN:");
                using (ConsoleExtensions.Indent())
                {
                    Write(action);
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
            }

            if (areEqual == expectFailure)
            {
                TestExecutionContext.CurrentContext.CurrentResult.
                    SetResult(ResultState.ChildFailure, "The emitted events were different from expected.", " ");
            }

            try
            {
                assert(ReplyAll());
            }
            catch (Exception)
            {
                if (expectFailure == false)
                    throw;
            }
        }

        static void Write(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
                Write(@event);
        }

        static string DiscoverName(Expression action)
        {
            var visitor = new TypeMethodCallExpressionVisitor(typeof(TState));
            visitor.Visit(action);
            return visitor.Method.Name;
        }

        static void Write(string item)
        {
            Console.WriteLine(item);
        }

        static void Write(IEvent @event)
        {
            Console.WriteLine($"- {@event}");
        }

        // ReSharper disable once ReturnTypeCanBeEnumerable.Local
        static byte[] Serialize(IEnumerable<IEvent> events)
        {
            var ms = new MemoryStream();
            byte i = 1;
            foreach (var @event in events)
            {
                var type = @event.GetType();
                var attr = type.GetCustomAttribute<EventTypeIdAttribute>();
                if (attr == null)
                    throw new ArgumentException($"Mark event '{type.FullName}' with {nameof(EventTypeIdAttribute)}");
                var id = attr.Id;
                ms.WriteByte(i);
                ms.Write(id.ToByteArray(), 0, 16);
                Serializer.Serialize(ms, @event);
            }

            return ms.ToArray();
        }
    }
}