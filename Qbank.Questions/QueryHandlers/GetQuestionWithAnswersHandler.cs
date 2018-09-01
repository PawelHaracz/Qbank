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
    public class GetQuestionWithAnswersHandler : IQueryHandler<GetQuestionWithAnswersQuery, QuestionWithAnswersState>
    {
        private readonly IEventStoreConnectionProvider _connectionProvider;

        public GetQuestionWithAnswersHandler(IEventStoreConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public async Task<QuestionWithAnswersState> HandleAsync(GetQuestionWithAnswersQuery query)
        {
            var streamId = $"{StreamPrefix.Question}_{query.User}";
            var aggregation = await _connectionProvider.Dispatch<QuestionWithAnswerProjection, QuestionWithAnswersState>(streamId).ConfigureAwait(false);
            if (!aggregation.Any())
            {
                return new QuestionWithAnswersState();
            }

            return aggregation.Single(e => e.Key == query.QuestionId.ToString()).Value;
        }
    }
}