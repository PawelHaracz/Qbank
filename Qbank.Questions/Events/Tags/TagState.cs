using System;
using System.Collections.Generic;
using Qbank.Core.Event;

namespace Qbank.Questions.Events.Tags
{
    public class TagState : BaseState
    {
        private readonly HashSet<string> _tagNameSet = new HashSet<string>();
        private readonly HashSet<Guid> _questionIdsSet = new HashSet<Guid>();

        public void Apply(TagCreated @event)
        {
            if (Has(@event.TagName) == false && Has(@event.QuestionId) == false)
            {
                _questionIdsSet.Add(@event.QuestionId);
                _tagNameSet.Add(@event.TagName);
            }
        }

        public bool Has(string tagName)
        {
            return _tagNameSet.Contains(tagName);
        }


        public bool Has(Guid questionId)
        {
            return _questionIdsSet.Contains(questionId);
        }

    }
}