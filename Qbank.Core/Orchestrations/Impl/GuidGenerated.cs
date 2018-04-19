using System;
using System.Runtime.Serialization;

namespace Qbank.Core.Orchestrations.Impl
{
    [DataContract]
    [EventTypeId("37294B23-62B1-4289-8E6F-7FED7FE9B627")]
    public class GuidGenerated : IEvent
    {
        [DataMember(Order = 1)]
        public readonly Guid Value;

        public GuidGenerated(Guid value)
        {
            Value = value;
        }
    }
}