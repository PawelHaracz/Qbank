using System.Runtime.Serialization;

namespace Qbank.Core.Orchestrations.Impl
{
    [DataContract]
    [EventTypeId("3B0F1B2A-9FED-4C7A-8CC7-4C17212D86A5")]
    public class OrchestrationEnded : IEvent
    {
        public static readonly OrchestrationEnded Instance = new OrchestrationEnded();
    }
}