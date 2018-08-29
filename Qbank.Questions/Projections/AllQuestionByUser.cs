using System;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Projections;
using Qbank.Questions.Events;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Projections.Models;

namespace Qbank.Questions.Projections
{
    public class AllQuestionByUser : ProjectionBase<AllQuestionByUser,  QuestionTeasersWith100Characters>
    {

        public override Guid Id => new Guid("B11E8157-BF5A-4CE3-850F-727CBD2AA450");
        protected override void Map(IEventMapping mapping)
        {
            //Expected streamId = Question_UserName
            mapping.For<QuestionCreated>((meta, e) =>meta.StreamId.Split('_')[1], Apply);
        }

        //Later change QuestionCreated to IEvent to handle many events :)
        static Task Apply(Metadata meta, QuestionCreated questionCreated, QuestionTeasersWith100Characters questionTeasersWith100Characters)
        {
            questionTeasersWith100Characters.Apply(questionCreated);
            return Task.CompletedTask;
        }
    }
}