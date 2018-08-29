using System;
using System.Collections.Generic;
using Qbank.Core.Event;

namespace Qbank.Questions.Events
{
    public class QuestionState : BaseState
    {
        private readonly HashSet<Guid> _questionGuidSet = new HashSet<Guid>();
        private readonly HashSet<(Guid QustionId, Guid AnswerId)> _answerGuidSet = new HashSet<(Guid QustionId, Guid AnswerId)>();
        public void Apply(QuestionCreated @event)
        {
            if (Has(@event.QuestionId) == false)
            {
                _questionGuidSet.Add(@event.QuestionId);
            }
        }

        public void Apply(AnswerCreated @event)
        {
            if (Has(@event.QuestionId, @event.AnswerId))
            {
                return;
            }

            _answerGuidSet.Add((@event.QuestionId, @event.AnswerId));
        }

        public bool Has(Guid questionId)
        {
            return _questionGuidSet.Contains(questionId);
        }

        public bool Has(Guid questionId, Guid answerId)
        {
            return _answerGuidSet.Contains((questionId, answerId));
        }
    }
}