using System;
using NUnit.Framework;
using Qbank.Questions;
using Qbank.Questions.Events;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Projections;
using Qbank.Questions.Projections.Models;

namespace Qbank.Test.Questions.Projections
{
    public class QuestionTest : ProjectionBaseTest<AllQuestionByUser, QuestionTeasersWith100Characters>
    {
        [Test]
        public void When_create_many_questions_with_diffrent_time_and_user_should_return_properly_questions_teaser()
        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();
            var g3 = Guid.NewGuid();
            var g4 = Guid.NewGuid();

            Given($"Question_PawelHaracz", new QuestionCreated(g1, "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s"));
            Given($"Question_PawelHaracz", new QuestionCreated(g2, "when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries"));
            Given($"Question_PawelHaracz", new QuestionCreated(g3, "but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages"));
            Given($"Question_JanKowalski", new QuestionCreated(g4, "and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum."));

            Then("PawelHaracz", new QuestionTeasersWith100Characters()
            {
                Questions =
               {
                   { g1, "Lorem Ipsum is simply dummy text of the printing and typesetting industry".Substring(0,20) },
                   { g2, "when an unknown printer took a galley of type and scrambled it to make a type".Substring(0,20)},
                   { g3,  "but also the leap into electronic typesetting, remaining essentially unchanged.".Substring(0,20)}
               }
            });
            Then("JanKowalski", new QuestionTeasersWith100Characters()
            {
                Questions =
                {
                    { g4,  "and more recently with desktop publishing software like Aldus Page".Substring(0,20) }
                }
            });
        }
    }
}