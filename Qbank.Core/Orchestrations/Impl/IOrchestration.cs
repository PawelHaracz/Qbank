using System.Collections.Generic;
using System.Threading.Tasks;
using Qbank.Core.Event;

namespace Qbank.Core.Orchestrations.Impl
{
    public interface IOrchestration
    {
        Task Execute(object input, IOrchestrationContext context, IEnumerable<IEvent> events, ICallSerializer serializer);
        Task FlushEvents();
    }
}