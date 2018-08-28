using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Qbank.Core.Event;

namespace Qbank.Core.Projections
{
    public static class Dispatcher
    {
        public static async Task<Dictionary<string, TState>> Dispatch<TProjection, TState>(this IEventStoreConnection connection, string streamId)
            where TProjection : ProjectionBase<TProjection, TState>, new() where TState : new()
        {

            var eventsSlice = await connection.ReadStreamEventsForwardAsync(streamId, 0, 100, false).ConfigureAwait(false);
            var events = eventsSlice.Events.Select(e => EventSerializer.Deserialize(e.Event.Data, new Guid(e.Event.EventType)));

            var result = new Dictionary<string, TState>();
            var p = new TProjection();
            foreach (var @event in events)
            {
                var type = @event.GetType();
                var attr = type.GetCustomAttribute<EventTypeIdAttribute>();
                if (attr == null)
                    throw new ArgumentException($"Mark e '{type.FullName}' with {nameof(EventTypeIdAttribute)}");
                var id = attr.Id;

                var dispatchedEvent = new DispatchedEvent
                {
                    Event = @event,
                    Metadata = new Metadata
                    {
                        EventId = Guid.NewGuid(),
                        EventTypeId = id,
                        StreamId = streamId
                    }
                };

                var partition = p.GetPartitioningKey(dispatchedEvent);
                if (result.TryGetValue(partition, out var state) == false)
                {
                    result[partition] = state = new TState();
                }

                await p.Dispatch(dispatchedEvent, state).ConfigureAwait(false);
            }
            return result;
        }
    }
}