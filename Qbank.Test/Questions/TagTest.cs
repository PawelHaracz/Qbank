using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Events.Tags;

namespace Qbank.Test.Questions
{
    [TestFixture]
    public class TagTest : BaseAggregateTest<TagState>
    {
        [Test]
        public void when_tag_exists_should_do_nothing()
        {
            var tagId = Guid.NewGuid();
            var tagName = "Test";

            Given(new CreatedTag(tagId, tagName));
            When(s => TagActions.Create(s, Guid.NewGuid(), tagName));
            Then();
        }

        [Test]
        public void when_tag_created_should_properly_create()
        {
            var tagId = Guid.NewGuid();
            var tagName = "Test";

            Given();
            When(s => TagActions.Create(s, tagId, tagName));
            Then(new CreatedTag(tagId, tagName));
        }

        [Test]
        public void add_many_tags_one_was_duplicated_by_name_should_skip_this_tag()
        {
            var ids = new Guid[6]
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            Given(new CreatedTag(ids[0], "test"));
            Given(new CreatedTag(ids[1], "test1"));
            Given(new CreatedTag(ids[2], "test2"));

            When(s => TagActions.Create(s, ids[3], "test"));
            When(s => TagActions.Create(s, ids[4], "test3"));
            When(s => TagActions.Create(s, ids[5], "test4"));

            Then(new CreatedTag(ids[4], "test3"));
            Then(new CreatedTag(ids[5], "test4"));
        }

        [Test]
        public void add_diffrent_tagName_but_the_same_name_should_do_nothing()
        {
            var tagId = Guid.NewGuid();

            Given(new CreatedTag(tagId, "test"));
            When(s => TagActions.Create(s, tagId, "test1"));
            Then();
        }
    }
}
