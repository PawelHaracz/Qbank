using System;
using NUnit.Framework;
using Qbank.Questions;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Events.Tags;
using Qbank.Questions.Projections;
using Qbank.Questions.Projections.States;

namespace Qbank.Test.Questions.Projections
{
    public class AllQuestionByTagProjectionTest : ProjectionBaseTest<AllQuestionByTagProjection, QuestionsByTagState>
    {
        [Test]
        public void Given_questions_tags_and_assosiated_these_should_grup_by_tag()
        {
            var g1 = Guid.NewGuid();
            var g2 = Guid.NewGuid();
            var g3 = Guid.NewGuid();
            var g4 = Guid.NewGuid();
            var g5 = Guid.NewGuid();
            var g6 = Guid.NewGuid();

            Given($"{StreamPrefix.Tag}_{g5}", new QuestionToTagAssosiated(g5, g1));
            Given($"{StreamPrefix.Tag}_{g5}", new QuestionToTagAssosiated(g5, g2));
            Given($"{StreamPrefix.Tag}_{g5}", new QuestionToTagAssosiated(g5, g3));
            Given($"{StreamPrefix.Tag}_{g6}", new QuestionToTagAssosiated(g6, g4));

            Then(g5.ToString(), new QuestionsByTagState()
            {
                Questions =
                {
                    g1,
                    g2,
                    g3
                }
            });
            Then(g6.ToString(), new QuestionsByTagState()
            {
                Questions =
                {
                    g4
                }
            });
        }       
    }
}