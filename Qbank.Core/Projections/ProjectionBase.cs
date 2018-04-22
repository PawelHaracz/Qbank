using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Qbank.Core.Projections
{
    /// <summary>
    /// Base class for projections, helping in double-dispatch of the events.
    /// </summary>
    /// <typeparam name="TProjection">The derived projection type</typeparam>
    /// <typeparam name="TState">The state type, created for every partition.</typeparam>
    public abstract class ProjectionBase<TProjection, TState> : IProjection
        where TProjection : ProjectionBase<TProjection, TState>
        where TState : new()
    {
        readonly EventMapping mapping;
        public abstract Guid Id { get; }
        
        protected ProjectionBase()
        {
            mapping = new EventMapping();
            // ReSharper disable once VirtualMemberCallInConstructor
            Map(mapping);
        }

        protected abstract void Map(IEventMapping mapping);

        public string GetPartitioningKey(DispatchedEvent dispatchedEvent)
        {
            return mapping.Map(dispatchedEvent);
        }

        public Type StateType => typeof(TState);

        public Task Dispatch(DispatchedEvent dispatchedEvent, object state)
        {
            return mapping.Dispatch(dispatchedEvent, (TState)state);
        }

        public IEnumerable<Guid> EventIds => mapping.EventIds;

        public interface IEventMapping
        {
            /// <summary>
            /// Declares projection as handling a specific dispatchedEvent.
            /// </summary>
            /// <typeparam name="TEvent"></typeparam>
            /// <param name="getPartitionKey"></param>
            /// <param name="apply"></param>
            void For<TEvent>(Func<Metadata, TEvent, string> getPartitionKey, Func<Metadata, TEvent, TState, Task> apply)
                where TEvent : IEvent;

            /// <summary>
            /// Declares projection as handling a specific dispatchedEvent.
            /// </summary>
            /// <typeparam name="TEvent"></typeparam>
            /// <param name="getPartitionKey"></param>
            /// <param name="apply"></param>
            void For<TEvent>(Func<TEvent, string> getPartitionKey, Func<TEvent, TState, Task> apply)
                where TEvent : IEvent;
        }

        class EventMapping : IEventMapping
        {
            readonly HashSet<Guid> eventIds = new HashSet<Guid>();

            readonly Dictionary<Guid, Func<DispatchedEvent, string>> partionKeyMappers =
                new Dictionary<Guid, Func<DispatchedEvent, string>>();

            readonly Dictionary<Guid, Func<DispatchedEvent, TState, Task>> appliers =
                new Dictionary<Guid, Func<DispatchedEvent, TState, Task>>();

            public void For<TEvent>(Func<Metadata, TEvent, string> getPartitionKey, Func<Metadata, TEvent, TState, Task> apply)
                where TEvent : IEvent
            {
                var id = typeof(TEvent).GetCustomAttribute<EventTypeIdAttribute>().Id;
                if (eventIds.Add(id) == false)
                {
                    throw new Exception($"The dispatchedEvent {typeof(TEvent)} has been already registered ");
                }

                partionKeyMappers[id] = (d) => getPartitionKey(d.Metadata, (TEvent)d.Event);
                appliers[id] = (d, s) => apply(d.Metadata, (TEvent)d.Event, s);
            }

            public void For<TEvent>(Func<TEvent, string> getPartitionKey, Func<TEvent, TState, Task> apply)
                where TEvent : IEvent
            {
                For<TEvent>((meta, e) => getPartitionKey(e), (m, e, s) => apply(e, s));
            }

            public IEnumerable<Guid> EventIds => eventIds;

            public string Map(DispatchedEvent e)
            {
                return partionKeyMappers[e.Metadata.EventTypeId](e);
            }

            public Task Dispatch(DispatchedEvent dispatchedEvent, TState state)
            {
                return appliers[dispatchedEvent.Metadata.EventTypeId](dispatchedEvent, state);
            }
        }
    }
}