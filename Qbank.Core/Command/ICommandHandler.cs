using System;
using System.Threading.Tasks;

namespace Qbank.Core.Command
{
    /// <summary>
    ///     Represents an asynchronous command handler (typically a database write operation).
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public interface ICommandHandler<in TCommand> where TCommand : ICommand
    {
        /// <summary>
        ///     Handles the specified command asynchronously.
        /// </summary>
        /// <param name="command">The command to handle.</param>
        Task<Guid> HandleAsync(TCommand command);
    }
}