using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Qbank.Core.Event;
using Qbank.Core.Projections;
using Qbank.Questions.Events.Tags;
using Qbank.Questions.Projections.States;

namespace Qbank.Questions.Projections
{
    public class AllQuestionByTagProjection : ProjectionBase<AllQuestionByTagProjection, QuestionsByTagState>
    {
        public override Guid Id => new Guid("115DB7A8-7735-4F9E-80C6-6B5DD7F727C3");
        protected override void Map(IEventMapping mapping)
        {
            mapping.For<AssosiatedQuestionToTag>((meta, e) => e.TagId.ToString(), Apply);
        }

        private Task Apply(Metadata metadata, AssosiatedQuestionToTag assosiatedQuestionToTag, QuestionsByTagState questionsByTagState)
        {
            questionsByTagState.Apply(assosiatedQuestionToTag);
            return Task.CompletedTask;
        }
    }
}
