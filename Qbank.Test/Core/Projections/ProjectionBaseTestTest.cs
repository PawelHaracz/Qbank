using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NUnit.Framework;
using Qbank.Core.Projections;

namespace Qbank.Test.Core.Projections
{
    public class ProjectionBaseTestTest : ProjectionBaseTest<ProjectionBaseTestTest.P, ProjectionBaseTestTest.State>
    {
        [Test]
        public void When_events_partitioned_Should_assert_properly()
        {
            Given("1", new ActionOccured("Test"));
            Given("2", new ActionOccured("Test"));
            Given("1", new ActionOccured("Test"));

            Then("Test1", new State { Value = 2 });
            Then("Test2", new State { Value = 1 });
        }

        public class P : ProjectionBase<P, State>
        {
            public override Guid Id { get; } = new Guid("74E9842A-48B0-4ABF-AF29-8FE0970BC75E");

            protected override void Map(IEventMapping mapping)
            {
                mapping.For<ActionOccured>((meta, e) => e.Name + meta.StreamId, Apply);
            }

            static Task Apply(Metadata meta, ActionOccured actionOccured, State s)
            {
                s.Value += 1;
                return Task.CompletedTask;
            }
        }

        [DataContract]
        public class State
        {
            [DataMember(Order = 1)]
            public int Value;
        }
    }
}