using System.Runtime.Serialization;

namespace Qbank.Core.Orchestrations.Impl
{
    [EventTypeId("5A325A2C-67ED-4951-85A9-B086E8A1BA83")]
    [DataContract]
    public class CallRecorded : IEvent
    {
        [DataMember(Order = 1)]
        public readonly byte[] Payload;

        [DataMember(Order = 2)]
        public readonly string ExceptionMessage;

        [DataMember(Order = 3)]
        public readonly string ExceptionStackTrace;

        public CallRecorded(byte[] payload)
        {
            Payload = payload;
        }

        public CallRecorded(string exceptionMessage, string exceptionStackTrace)
        {
            ExceptionMessage = exceptionMessage;
            ExceptionStackTrace = exceptionStackTrace;
        }
    }
}