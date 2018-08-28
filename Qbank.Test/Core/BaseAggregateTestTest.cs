using System;
using System.Collections.Generic;
using NUnit.Framework;
using Qbank.Core;
using Qbank.Core.Event;

namespace Qbank.Test.Core
{
    public class BaseAggregateTestTest : BaseAggregateTest<BaseAggregateTestTest.State>
    {
        [Test]
        public void When_no_event_emitted_Should_properly_assert_empty()
        {
            Given(new ActionOccured("test"));
            When(s => Actions.Occur(s, "test"));
            Then();
        }

        [Test]
        public void When_event_emitted_Should_properly_assert()
        {
            Given();
            When(s => Actions.Occur(s, "test"));
            Then(new ActionOccured("test"));
        }

        [Test]
        public void When_events_emitted_Should_properly_assert()
        {
            Given();
            When(s => Actions.Occur(s, "test"));
            When(s => Actions.Occur(s, "test1"));
            Then(new ActionOccured("test"), new ActionOccured("test1"));
        }

        [Test]
        public void When_events_emitted_wrongly_Should_throw()
        {
            Given();
            When(s => Actions.Occur(s, "test"));
            Then();

            ExpectFailure();
        }


        [Test]
        public void Can_assert_state()
        {
            Given();
            When(s => Actions.Occur(s, "test"));
            Then(s => throw new Exception());

            ExpectFailure();
        }

        public class State : BaseState
        {
            readonly HashSet<string> names = new HashSet<string>();

            public void Apply(ActionOccured e)
            {
                names.Add(e.Name);
            }

            public bool Has(string name)
            {
                return names.Contains(name);
            }
        }

        public static class Actions
        {
            public static IEnumerable<IEvent> Occur(State state, string name)
            {
                if (state.Has(name) == false)
                    yield return new ActionOccured(name);
            }
        }
    }
}