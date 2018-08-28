using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qbank.Core.Event
{
    public interface IEventStoreConnectionProvider
    {
        Task Execute<TState>(string aggregatedId, Func<TState, IEnumerable<IEvent>> execute) where TState : BaseState, new();
    }
}