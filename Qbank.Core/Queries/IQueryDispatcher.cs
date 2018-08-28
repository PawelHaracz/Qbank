using System.Threading.Tasks;

namespace Qbank.Core.Queries
{
    /// <summary>
    /// Finds the correct asynchronous query handler and invokes it.
    /// </summary>
    public interface IQueryDispatcher
    {
        /// <summary>
        /// Processes the specified query asynchronously by finding the appropriate handler and
        /// invoking it.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="query">The query to process.</param>
        /// <returns>The query result.</returns>
        Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query);
    }
}