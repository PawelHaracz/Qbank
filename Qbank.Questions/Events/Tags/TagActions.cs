using System;
using System.Collections.Generic;
using Qbank.Core.Event;
using Qbank.Questions.Events.Questions;

namespace Qbank.Questions.Events.Tags
{
    public class TagActions
    {
        public static IEnumerable<IEvent> Create(QuestionState state, Guid questionId, string tagName)
        {
            if (state.Has(tagName, questionId) == false)
            {
                yield return new TagCreated(questionId, tagName);
            }
        }
    }
}
