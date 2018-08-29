using System;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Projections;
using Qbank.Questions.Events;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Projections.Models;

namespace Qbank.Questions.Projections
{
    public class AllQuestionByUser : ProjectionBase<AllQuestionByUser,  QuestionTeasersWith20Characters>
    {

        public override Guid Id => new Guid("B11E8157-BF5A-4CE3-850F-727CBD2AA450");
        protected override void Map(IEventMapping mapping)
        {
            //Expected streamId = Question_DateTime:yy-MM-dd:HH-mm-ss_UserName
            mapping.For<QuestionCreated>((meta, e) =>meta.StreamId.Split('_')[2], Apply);
        }

        //Later change QuestionCreated to IEvent to handle many events :)
        static Task Apply(Metadata meta, QuestionCreated questionCreated, QuestionTeasersWith20Characters questionTeasersWith20Characters)
        {
            questionTeasersWith20Characters.Apply(questionCreated);
            return Task.CompletedTask;
        }
    }
}