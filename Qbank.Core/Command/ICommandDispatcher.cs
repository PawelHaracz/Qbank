using System.Threading.Tasks;
using System;
namespace Qbank.Core.Command
{
    /// <summary>
    ///     Finds the correct asynchronous command handler and invokes it.
    /// </summary>
    public interface ICommandDispatcher
    {
        /// <summary>
        ///     Processes the specified command asychronously by finding the appropriate handler and invoking it.
        /// </summary>
        /// <param name="command">The command to process.</param>
        /// <returns>Command Id.</returns>
        Task<Guid> DispatchAsync<T>(T command) where T : ICommand;
    }
}