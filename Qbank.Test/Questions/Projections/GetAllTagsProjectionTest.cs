using System;
using NUnit.Framework;
using Qbank.Questions;
using Qbank.Questions.Events.Tags;
using Qbank.Questions.Projections;
using Qbank.Questions.Projections.States;

namespace Qbank.Test.Questions.Projections
{
    public class GetAllTagsProjectionTest : ProjectionBaseTest<GetAllTagsProjection, TagNameState>
    {
        [Test]
        public void Given_three_tags_should_give_all()
        {
            var event1 = new TagCreated(Guid.NewGuid(), "Test1");
            var event2 = new TagCreated(Guid.NewGuid(), "Test2");
            var event3 = new TagCreated(Guid.NewGuid(), "Test3");

            Given($"{StreamPrefix.Question}", event1);
            Given($"{StreamPrefix.Question}", event2);
            Given($"{StreamPrefix.Question}", event3);

            Then(event1.QuestionId.ToString(), new TagNameState { Name = event1.TagName });
            Then(event2.QuestionId.ToString(), new TagNameState { Name = event2.TagName });
            Then(event3.QuestionId.ToString(), new TagNameState { Name = event3.TagName });
        }
    }
}