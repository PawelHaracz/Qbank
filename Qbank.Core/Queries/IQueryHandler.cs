﻿using System.Threading.Tasks;

namespace Qbank.Core.Queries
{
    /// <summary>
    ///     Represents an asynchronous query handler (typically a database read operation).
    /// </summary>
    /// <typeparam name="TQuery">The type of the query.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public interface IQueryHandler<in TQuery, TResult> where TQuery : IQuery<TResult>
    {
        /// <summary>
        ///     Handles the specified query asynchronously.
        /// </summary>
        /// <param name="query">The query to handle.</param>
        /// <returns>The query results.</returns>
        Task<TResult> HandleAsync(TQuery query);
    }
}