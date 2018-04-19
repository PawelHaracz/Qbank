using System.Runtime.Serialization;

namespace Qbank.Core.Orchestrations.Impl
{
    [EventTypeId("E4BBD014-7718-4010-9257-866308BEBAE6")]
    [DataContract]
    public class InputCallRecorded : IEvent
    {
        [DataMember(Order = 1)]
        public readonly byte[] Payload;

        public InputCallRecorded(byte[] payload)
        {
            Payload = payload;
        }
    }
}