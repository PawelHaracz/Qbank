using System;
using System.Threading.Tasks;
using Autofac;

namespace Qbank.Core.Queries
{
    public class QueryDispatcher : IQueryDispatcher
    {
        private readonly IComponentContext _context;

        public QueryDispatcher(IComponentContext context)
        {
            _context = context;
        }

        public async Task<TResult> DispatchAsync<TResult>(IQuery<TResult> query)
        {
            if (query == null)
            {
                throw new ArgumentNullException(
                    $"Command: '{typeof(TResult).Name}' can not be null.");
            }

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(TResult));
            dynamic handler = _context.Resolve(handlerType);

            return await handler.HandleAsync((dynamic) query);
        }
    }
}