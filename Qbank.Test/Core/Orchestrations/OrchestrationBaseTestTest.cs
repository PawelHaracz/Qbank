using System;
using System.Threading.Tasks;
using NUnit.Framework;
using Qbank.Core.Orchestrations;

namespace Qbank.Test.Core.Orchestrations
{
    public class OrchestrationBaseTestTest : OrchestrationBaseTest<OrchestrationBaseTestTest.GuidOrchestration, int>
    {
        public static readonly Guid CallId = Guid.NewGuid();
        public static readonly Guid Param1 = Guid.NewGuid();

        [Test]
        public void Given()
        {
            Given(0);
            NextGuid(Param1);
            NextGuid(CallId);
        }

        public class GuidOrchestration : Orchestration<int>
        {
            private readonly Func<Guid, Guid, Task<int>> _serviceCall;

            public GuidOrchestration(Func<Guid, Guid, Task<int>> serviceCall)
            {
                this._serviceCall = serviceCall;
            }

            protected override Task Execute(int input)
            {
                var parameter = NewGuid();
                return Call(callId => _serviceCall(callId, parameter));
            }
        }

        protected override GuidOrchestration Build() => new GuidOrchestration((Guid callId, Guid parameter) =>
        {
            Assert.AreEqual(CallId, callId);
            Assert.AreEqual(Param1, parameter);
            return Task.FromResult(1);
        });
    }
}