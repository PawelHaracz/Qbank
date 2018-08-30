using System;
using System.Collections.Generic;
using Qbank.Core.Event;

namespace Qbank.Questions.Events.Tags
{
    public class TagActions
    {
        public static IEnumerable<IEvent> Create(TagState state, Guid tagId, string tagName)
        {
            if (state.Has(tagName) == false && state.Has(tagId) == false)
            {
                yield return new TagCreated(tagId, tagName);
            }
        }

        public static IEnumerable<IEvent> Assosiate(TagState state, Guid tagId, Guid questionId)
        {
            if (state.Has(tagId, questionId) == false)
            {
                yield return new AssosiatedQuestionToTag(tagId, questionId);
            }
        }
    }
}
