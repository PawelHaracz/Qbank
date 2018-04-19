using System;
using System.Runtime.Serialization;

namespace Qbank.Core.Orchestrations.Impl
{
    [DataContract]
    [EventTypeId("D952AF54-569C-44EF-A840-31F8394C4199")]
    public class DateTimeRetrieved : IEvent
    {
        [DataMember(Order = 1)]
        public readonly DateTimeOffset Value;

        public DateTimeRetrieved(DateTimeOffset value)
        {
            Value = value;
        }
    }
}