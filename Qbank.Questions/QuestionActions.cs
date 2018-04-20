using System;
using System.Collections.Generic;
using Qbank.Core;

namespace Qbank.Questions
{
    public class QuestionActions
    {
        public static IEnumerable<IEvent> Create(QuestionState state, Guid questionId, string question)
        {
            if (state.Has(questionId) == false)
            {
                yield return new QuestionCreated(questionId, question);
            }
        }

        public static IEnumerable<IEvent> Create(QuestionState state, Guid questionId, Guid answerId, string answer, bool isCorrect)
        {
            if (state.Has(questionId)== true && state.Has(questionId, answerId) == false)
            {
                yield return new AnswerCreated(answerId, questionId, answer, isCorrect);
            }
        }
    }
}