using System;
using System.Threading.Tasks;
using Qbank.Core.Projections;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Events.Tags;
using Qbank.Questions.Projections.States;

namespace Qbank.Questions.Projections
{
    public class AllQuestionByUserProjection : ProjectionBase<AllQuestionByUserProjection,  QuestionTeasersWith100CharactersState>
    {

        public override Guid Id => new Guid("B11E8157-BF5A-4CE3-850F-727CBD2AA450");
        protected override void Map(IEventMapping mapping)
        {
          
            //Expected streamId = Question_UserName
            mapping.For<QuestionCreated>((meta, e) =>e.QuestionId.ToString(), Apply);
            mapping.For<AnswerCreated>((created) => string.Empty, (created, state) => Task.CompletedTask);
            mapping.For<TagCreated>(created => string.Empty, (created, state) => Task.CompletedTask);
        }

        //Later change QuestionCreated to IEvent to handle many events :)
        static Task Apply(Metadata meta, QuestionCreated questionCreated, QuestionTeasersWith100CharactersState questionTeasersWith100Characters)
        {
            questionTeasersWith100Characters.Apply(questionCreated);
            return Task.CompletedTask;
        }
    }
} 