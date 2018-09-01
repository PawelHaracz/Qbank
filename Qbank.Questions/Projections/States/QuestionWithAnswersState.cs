using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Events.Tags;

namespace Qbank.Questions.Projections.States
{
    [DataContract]
    public class QuestionWithAnswersState
    {
        [DataMember(Order = 1)]
        public string Question { get; set; }

        [DataMember(Order = 2)]
        public HashSet<Answer> Answers { get; set; } = new HashSet<Answer>();

        [DataMember(Order = 3)]
        public HashSet<string> Tags = new HashSet<string>();

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
        public void Apply(TagCreated tagCreated)
        {
            Tags.Add(tagCreated.TagName);
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
