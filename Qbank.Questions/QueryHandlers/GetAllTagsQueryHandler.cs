using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Queries;
using Qbank.Questions.Projections;
using Qbank.Questions.Projections.States;
using Qbank.Questions.Quries;

namespace Qbank.Questions.QueryHandlers
{
    public class GetAllTagsQueryHandler : IQueryHandler<GetAllTagsQuery, IDictionary<Guid, string>>
    {
        private readonly IEventStoreConnectionProvider _connectionProvider;

        public GetAllTagsQueryHandler(IEventStoreConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<IDictionary<Guid, string>> HandleAsync(GetAllTagsQuery query)
        {
            var streamId = $"{StreamPrefix.Tag}";
            var aggregation = await _connectionProvider.Dispatch<GetAllTagsProjection, TagNameState>(streamId).ConfigureAwait(false);
            if (!aggregation.Any())
            {
                return new Dictionary<Guid, string>();
            }

            return aggregation.Select(kv => new KeyValuePair<Guid, string>(Guid.Parse(kv.Key), kv.Value.Name)).ToDictionary(pair => pair.Key,pair => pair.Value);
        }
    }
}