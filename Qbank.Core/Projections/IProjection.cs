using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Qbank.Core.Projections
{
    public interface IProjection
    {
        /// <summary>
        /// Unique id of this projection.
        /// </summary>
        Guid Id { get; }

        /// <summary>
        /// Gets a partitioning key, <see cref="string.Empty"/> if there's only one partition.
        /// </summary>
        /// <param name="dispatchedEvent"></param>
        /// <returns></returns>
        string GetPartitioningKey(DispatchedEvent dispatchedEvent);

        /// <summary>
        /// Gets the state type.
        /// </summary>
        Type StateType { get; }

        /// <summary>
        /// Gets the event identifiers <see cref="EventTypeIdAttribute.Id"/> that are handled by this projection.
        /// </summary>
        IEnumerable<Guid> EventIds { get; }

        /// <summary>
        /// Dispatches the <see cref="dispatchedEvent"/> onto <paramref name="state"/>.
        /// </summary>
        Task Dispatch(DispatchedEvent dispatchedEvent, object state);
    }
}