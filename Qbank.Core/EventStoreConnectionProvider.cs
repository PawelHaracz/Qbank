using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.SystemData;
using Qbank.Core.Configuration;
using Microsoft.Extensions.Options;
using Qbank.Core.Projections;
using Qbank.Core.Event;

namespace Qbank.Core
{
    public class EventStoreConnectionProvider : IEventStoreConnectionProvider, IDisposable
    {
        private static long _expectedVersion = ExpectedVersion.EmptyStream;
        private readonly EventStoreConfiguration _configuration;    
        private  IEventStoreConnection _connection;

        public EventStoreConnectionProvider(IOptions<EventStoreConfiguration> configuration)
        {
            _configuration = configuration.Value;

            var settings = ConnectionSettings.Create()
                .SetDefaultUserCredentials(new UserCredentials(_configuration.UserName, _configuration.UserPassword))
                .SetHeartbeatTimeout(new TimeSpan(500))
                .Build();
            _connection = EventStoreConnection.Create(settings, new IPEndPoint(IPAddress.Parse(_configuration.Ip), _configuration.Port));
            if (_connection == null)
            {
                throw new EventStoreConnectionException($"Cannot connect to EventStore using: {_configuration.UserName} and connection to {_configuration.Ip}:{_configuration.Port}");
            }
            _connection.ConnectAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        }

        public async Task Execute<TState>(string streamId, Func<TState, IEnumerable<IEvent>> execute) where TState : BaseState, new()
        {
            TState state;
            long version;
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

            var actualVersion = await Execute(streamId, version, state, execute).ConfigureAwait(false);

            if (actualVersion > version && actualVersion > _configuration.SnapchotLimit)
            {
                Snapshot<TState>.AddSnapshot(streamId, new Snapshot<TState>(state, actualVersion));
            }
        }

        public async Task<Dictionary<string, TState>> Dispatch<TProjection, TState>(string streamId) where TProjection : ProjectionBase<TProjection, TState>, new() where TState : new()
        {
            Dictionary<string, TState> result;
            long version;
            if (SnapshotProjection<TState>.TryGetSnapshot(streamId, out var snapshot))
            {
                result = snapshot.State;
                version = snapshot.Version;
            }
            else
            {
                result = new Dictionary<string, TState>();
                version = 0;
            }

            var actualVersion = await Dispatch<TProjection, TState>(streamId, version, result).ConfigureAwait(false);

            if (actualVersion > version && actualVersion > _configuration.SnapchotLimit)
            {
                SnapshotProjection<TState>.AddSnapshot(streamId, new SnapshotProjection<Dictionary<string, TState>>(result, actualVersion));
            }
            return result;
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
        }

        private async Task<long> Dispatch<TProjection, TState>(string streamId, long version, Dictionary<string, TState> result) where TProjection : ProjectionBase<TProjection, TState>, new() where TState : new()
        {
            StreamEventsSlice eventsSlice;
            var p = new TProjection();

            do
            {
                eventsSlice = await _connection.ReadStreamEventsForwardAsync(streamId, version, _configuration.SnapchotLimit, false).ConfigureAwait(false);
                var events = eventsSlice.Events.Select(e => EventSerializer.Deserialize(e.Event.Data, new Guid(e.Event.EventType)));

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
                    if (string.IsNullOrWhiteSpace(partition) == false)
                    {
                        if (result.TryGetValue(partition, out var state) == false)
                        {
                            result[partition] = state = new TState();
                        }

                        await p.Dispatch(dispatchedEvent, state).ConfigureAwait(false);
                    }

                    version++;
                }
                

            } while (!eventsSlice.IsEndOfStream);

            return version;
        }

        private async Task<long> Execute<TState>(string streamId, long version, TState state,
            Func<TState, IEnumerable<IEvent>> execute)
            where TState : BaseState, new()
        {

            StreamEventsSlice eventsSlice;
            do
            {
                
                eventsSlice = await _connection.ReadStreamEventsForwardAsync(streamId, version, _configuration.SnapchotLimit, false).ConfigureAwait(false);
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
                var eventTypeId = e.GetType().GetCustomAttribute<EventTypeIdAttribute>().Id.ToString();
                var data = EventSerializer.Serialize(e);
                return new EventData(eventId, eventTypeId, false, data, new byte[0]);
            }).ToArray();

            
            var status = await _connection.AppendToStreamAsync(streamId, ExpectedVersion.Any /*Interlocked.Read(ref _expectedVersion)*/, newEventsData).ConfigureAwait(false);
            Interlocked.Add(ref _expectedVersion, status.NextExpectedVersion);

            return version;
        }
    }
}