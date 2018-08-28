using System;
using System.Threading.Tasks;
using Qbank.Core.Event;

namespace Qbank.Core.Orchestrations.Impl
{
    public interface IOrchestrationContext
    {
        /// <summary>
        /// Generates new Guid like <see cref="Guid.NewGuid"/>.
        /// </summary>
        Guid NewGuid();

        /// <summary>
        /// Gets the current date like <see cref="DateTimeOffset.UtcNow"/>.
        /// </summary>
        /// <returns></returns>
        DateTimeOffset DateTimeUtcNow();

        /// <summary>
        /// Notifies the context that the current execution is postponed and scheduled with a <see cref="Orchestration{TInput}.Delay"/> for the future.
        /// </summary>
        void EndCurrentExecution();

        /// <summary>
        /// Appends event to the context.
        /// </summary>
        void Append(IEvent @event);

        /// <summary>
        /// Flushes the context.
        /// </summary>
        /// <returns></returns>
        Task Flush();
    }
}