using System;
using Qbank.Core.Queries;
using Qbank.Questions.Projections.States;

namespace Qbank.Questions.Quries
{
    public class GetQuestionWithAnswersQuery : IQuery<QuestionWithAnswersState>
    {
        public Guid QuestionId { get; set;}
    }
}