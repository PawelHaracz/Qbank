using System;
using System.Collections.Generic;
using Qbank.Core.Event;

namespace Qbank.Questions.Events.Tags
{
    public class TagState : BaseState
    {
        private readonly HashSet<string> _tagNameSet = new HashSet<string>();
        private readonly HashSet<Guid> _tagIdsSet = new HashSet<Guid>();
        private readonly HashSet<(Guid TagId, Guid QuestionId)> _tagQuestionSet =new HashSet<(Guid TagId, Guid QuestionId)>(); 

        public void Apply(TagCreated @event)
        {
            if (Has(@event.TagName) == false && Has(@event.TagId) == false)
            {
                _tagIdsSet.Add(@event.TagId);
                _tagNameSet.Add(@event.TagName);
            }
        }

        public void Apply(AssosiatedQuestionToTag @event)
        {

        }

        public bool Has(string tagName)
        {
            return _tagNameSet.Contains(tagName);
        }

        public bool Has(Guid tagId, Guid questionId)
        {
            return _tagQuestionSet.Contains((tagId, questionId));
        }

        public bool Has(Guid tagId)
        {
            return _tagIdsSet.Contains(tagId);
        }

    }
}