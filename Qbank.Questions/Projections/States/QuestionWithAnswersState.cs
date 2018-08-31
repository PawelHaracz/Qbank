using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Qbank.Questions.Events.Questions;

namespace Qbank.Questions.Projections.States
{
    [DataContract]
    public class QuestionWithAnswersState
    {
        public string Question { get; set; }
        public IList<Answer> Answers { get; set; } = new List<Answer>();

        public void Apply(QuestionCreated questionCreated)
        {
            Question = questionCreated.Question;
        }

        public void Apply(AnswerCreated answerCreated)
        {
            Answers.Add(new Answer
            {
                Name = answerCreated.Answer,
                Id = answerCreated.AnswerId,
                IsCorrect = answerCreated.IsCorrect
            });
        }
    }

    [DataContract]
    public class Answer
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }
        [DataMember(Order = 2)]
        public Guid Id { get; set; }
        [DataMember(Order = 3)]
        public bool IsCorrect { get; set; }
    }
}
