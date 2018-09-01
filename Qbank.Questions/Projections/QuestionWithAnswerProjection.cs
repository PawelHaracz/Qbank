using System;
using System.Threading.Tasks;
using Qbank.Core.Projections;
using Qbank.Questions.Commands;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Events.Tags;
using Qbank.Questions.Projections.States;

namespace Qbank.Questions.Projections
{
    public class QuestionWithAnswerProjection : ProjectionBase<QuestionWithAnswerProjection, QuestionWithAnswersState>
    {
        public override Guid Id => new Guid("1B936ADB-6F2E-4FA7-B440-0923853C42C7");

        protected override void Map(IEventMapping mapping)
        {
            mapping.For<QuestionCreated>((meta, e) => e.QuestionId.ToString(), Apply);
            mapping.For<AnswerCreated>((meta, e) => e.QuestionId.ToString(), Apply);
            mapping.For<TagCreated>((meta, e) => e.QuestionId.ToString(), Apply);
        }

        private Task Apply(Metadata metadata, TagCreated tagCreated, QuestionWithAnswersState state)
        {
            state.Apply(tagCreated);
            return Task.CompletedTask;
        }

        private Task Apply(Metadata metadata, QuestionCreated questionCreated, QuestionWithAnswersState state)
        {
            state.Apply(questionCreated);
            return Task.CompletedTask;
        }

        private Task Apply(Metadata metadata, AnswerCreated answerCreated, QuestionWithAnswersState state)
        {
            state.Apply(answerCreated);
            return Task.CompletedTask;
        }
    }
}