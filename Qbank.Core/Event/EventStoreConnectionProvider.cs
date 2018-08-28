using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using EventStore.ClientAPI.Exceptions;
using EventStore.ClientAPI.SystemData;
using Qbank.Core.Configuration;
using Microsoft.Extensions.Options;

namespace Qbank.Core.Event
{
    public class EventStoreConnectionProvider : IEventStoreConnectionProvider, IDisposable
    {
        private readonly EventStoreConfiguration _configuration;

        private readonly IEventStoreConnection _connection;

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

            var actualVersion = await Execute(streamId, version, state, execute).ConfigureAwait(false);

            if (actualVersion > 1000)
            {
                Snapshot<TState>.AddSnapshot(streamId, new Snapshot<TState>(state, actualVersion));
            }
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }

        private async Task<int> Execute<TState>(string streamId, int version, TState state,
            Func<TState, IEnumerable<IEvent>> execute)
            where TState : BaseState, new()
        {
            StreamEventsSlice eventsSlice;        
            do
            {
                eventsSlice = await _connection.ReadStreamEventsForwardAsync(streamId, version, 1000, false).ConfigureAwait(false);

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
                var eventTypeId = CustomAttributeExtensions.GetCustomAttribute<EventTypeIdAttribute>((MemberInfo) e.GetType()).Id.ToString();
                var data = EventSerializer.Serialize(e);
                return new EventData(eventId, eventTypeId, false, data, new byte[0]);
            }).ToArray();

            await _connection.AppendToStreamAsync(streamId, ExpectedVersion.Any, newEventsData).ConfigureAwait(false);

            return version;
        }
    }
}