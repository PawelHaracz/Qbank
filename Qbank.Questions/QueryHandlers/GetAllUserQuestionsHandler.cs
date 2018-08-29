using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Event;
using Qbank.Core.Queries;
using Qbank.Questions.Projections;
using Qbank.Questions.Projections.Models;
using Qbank.Questions.Quries;

namespace Qbank.Questions.QueryHandlers
{
    public class GetAllUserQuestionsHandler : IQueryHandler<GetAllUserQuestions, IDictionary<Guid, string>>
    {
        private readonly IEventStoreConnectionProvider _connectionProvider;

        public GetAllUserQuestionsHandler(IEventStoreConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<IDictionary<Guid,string>> HandleAsync(GetAllUserQuestions query)
        {
            var streamId = $"{StreamPrefix.Question}_{query.User}";
            var aggregation = await _connectionProvider.Dispatch<AllQuestionByUser, QuestionTeasersWith100Characters>(streamId).ConfigureAwait(false);
            if (!aggregation.Any())
            {
                return new Dictionary<Guid, string>();
            }
            return aggregation.Single().Value.Questions;
        }
    }
}