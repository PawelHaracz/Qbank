using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;

namespace Qbank.Core
{
    public static class Executor
    {
        public static async Task Execute<TState>(this IEventStoreConnection connection, string streamId,
            Func<TState, IEnumerable<IEvent>> execute)
            where TState : BaseState, new()
        {
            await connection.Execute(streamId, 0, new TState(), execute).ConfigureAwait(false);
        }

        public static async Task ExecuteWithSnapshot<TState>(this IEventStoreConnection connection, string streamId,
            Func<TState, IEnumerable<IEvent>> execute)
            where TState : BaseState, new()
        {
            TState state;
            int version;
            if (Snapshot<TState>.TryGetSnapshot(streamId, out var snapshot))
            {
                state = snapshot.State;
                version = snapshot.Version;
            }
            else
            {
                state = new TState();
                version = 0;
            }

            var actualVersion = await connection.Execute(streamId, version, state, execute).ConfigureAwait(false);

            if (actualVersion > 1000)
            {
                Snapshot<TState>.AddSnapshot(streamId, new Snapshot<TState>(state, actualVersion));
            }
        }

        private static async Task<int> Execute<TState>(this IEventStoreConnection connection, string streamId, int version, TState state,
            Func<TState, IEnumerable<IEvent>> execute)
            where TState : BaseState, new()
        {
            StreamEventsSlice eventsSlice;
            do
            {
                eventsSlice = await connection.ReadStreamEventsForwardAsync(streamId, version, 1000, false).ConfigureAwait(false);

                foreach (var e in eventsSlice.Events)
                {
                    var @event = EventSerializer.Deserialize(e.Event.Data, new Guid(e.Event.EventType));
                    StateApplier.Apply(state, @event);
                    version++;
                }
            } while (!eventsSlice.IsEndOfStream);

            var newEvents = execute(state);

            var newEventsData = newEvents.Select(e =>
            {
                var eventId = Guid.NewGuid();
                var eventTypeId = e.GetType().GetCustomAttribute<EventTypeIdAttribute>().Id;
                var data = EventSerializer.Serialize(e);
                return new EventData(eventId, $"{eventTypeId}", false, data, new byte[0]);
            }).ToArray();

            await connection.AppendToStreamAsync(streamId, ExpectedVersion.Any, newEventsData).ConfigureAwait(false);

            return version;
        }
    }

    public class Snapshot<TState>
    {
        public static ConcurrentDictionary<string, Snapshot<TState>> SnapshotDictionary = new ConcurrentDictionary<string, Snapshot<TState>>();
        public Snapshot(TState state, int version)
        {
            State = state;
            Version = version;
        }

        public TState State { get; }
        public int Version { get; }

        public static bool TryGetSnapshot(string key, out Snapshot<TState> snapshot) => SnapshotDictionary.TryGetValue(key, out snapshot);

        public static void AddSnapshot(string key, Snapshot<TState> snapshot) => SnapshotDictionary.AddOrUpdate(key, snapshot, (_, __) => snapshot);
    }
}