using System;

namespace Qbank.Core.Projections
{
    public class DispatchedEvent
    {
        public Metadata Metadata;
        public IEvent Event;
    }

    public class Metadata
    {
        /// <summary>
        /// The identifier provided by <see cref="EventTypeIdAttribute.Id"/> of the <see cref="IEvent"/> implementor
        /// </summary>
        public Guid EventTypeId;

        /// <summary>
        /// The instance specific event id.
        /// </summary>
        public Guid EventId;

        /// <summary>
        /// The identifier of the stream that the event belongs to.
        /// </summary>
        public string StreamId;
    }
}