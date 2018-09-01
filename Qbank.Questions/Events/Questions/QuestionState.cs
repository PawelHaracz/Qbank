using System;
using System.Collections.Generic;
using Qbank.Core.Event;
using Qbank.Questions.Events.Tags;

namespace Qbank.Questions.Events.Questions
{
    public class QuestionState : BaseState
    {
        private readonly HashSet<Guid> _questionGuidSet = new HashSet<Guid>();
        private readonly HashSet<(Guid QustionId, Guid AnswerId)> _answerGuidSet = new HashSet<(Guid QustionId, Guid AnswerId)>();
        private readonly HashSet<(string TagName, Guid QuestionId)> _tagNamesSet = new HashSet<(string TagName, Guid QuestionId)>();
        public void Apply(QuestionCreated @event)
        {
            if (Has(@event.QuestionId) == false)
            {
                _questionGuidSet.Add(@event.QuestionId);
            }
        }

        public void Apply(AnswerCreated @event)
        {
            if (Has(@event.QuestionId) && Has(@event.QuestionId, @event.AnswerId) == false)
            {
                _answerGuidSet.Add((@event.QuestionId, @event.AnswerId));
            }            
        }

        public void Apply(TagCreated @event)
        {
            if (Has(@event.TagName, @event.QuestionId) == false)
            {
                _tagNamesSet.Add((@event.TagName, @event.QuestionId));
            }
        }

        public bool Has(Guid questionId)
        {
            return _questionGuidSet.Contains(questionId);
        }

        public bool Has(Guid questionId, Guid answerId)
        {
            return _answerGuidSet.Contains((questionId, answerId));
        }

        public bool Has(string tagName, Guid questionId)
        {
            return _tagNamesSet.Contains((tagName, questionId));
        }
    }
}