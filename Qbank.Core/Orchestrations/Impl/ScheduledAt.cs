using System;
using System.Runtime.Serialization;
using Qbank.Core.Event;

namespace Qbank.Core.Orchestrations.Impl
{
    [DataContract]
    [EventTypeId("37492F15-B900-428B-A5C7-0AF58B1BA51D")]
    public class ScheduledAt : IEvent
    {
        [DataMember(Order = 1)]
        public readonly DateTimeOffset Value;

        public ScheduledAt(DateTimeOffset value)
        {
            Value = value;
        }
    }
}