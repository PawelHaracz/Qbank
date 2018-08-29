using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Qbank.Core.Projections;
using Qbank.Core.Event;
namespace Qbank.Core
{
    public interface IEventStoreConnectionProvider
    {
        Task Execute<TState>(string aggregatedId, Func<TState, IEnumerable<IEvent>> execute) where TState : BaseState, new();

        Task<Dictionary<string, TState>> Dispatch<TProjection, TState>(string streamId)
            where TProjection : ProjectionBase<TProjection, TState>, new() where TState : new();
    }
}