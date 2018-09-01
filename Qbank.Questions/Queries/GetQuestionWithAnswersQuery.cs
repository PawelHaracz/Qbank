using System;
using Qbank.Core.Queries;
using Qbank.Questions.Projections.States;

namespace Qbank.Questions.Quries
{
    public class GetQuestionWithAnswersQuery : IQuery<QuestionWithAnswersState>
    {
        public string User { get; set; }
        public Guid QuestionId { get; set; }
    }
}