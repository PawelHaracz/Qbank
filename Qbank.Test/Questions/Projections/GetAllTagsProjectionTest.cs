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
            var event1 = new CreatedTag(Guid.NewGuid(), "Test1");
            var event2 = new CreatedTag(Guid.NewGuid(), "Test2");
            var event3 = new CreatedTag(Guid.NewGuid(), "Test3");

            Given($"{StreamPrefix.Tag}", event1);
            Given($"{StreamPrefix.Tag}", event2);
            Given($"{StreamPrefix.Tag}", event3);

            Then(event1.TagId.ToString(), new TagNameState { Name = event1.TagName });
            Then(event2.TagId.ToString(), new TagNameState { Name = event2.TagName });
            Then(event3.TagId.ToString(), new TagNameState { Name = event3.TagName });
        }
    }
}