using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Qbank.Questions.Events.Questions;

namespace Qbank.Questions.Projections.States
{
    [DataContract]
    public class QuestionTeasersWith100CharactersState
    {  
        [DataMember(Order = 1)]
        public Dictionary<Guid, string> Questions { get; set; } = new Dictionary<Guid, string>();  

        
        public void Apply(QuestionCreated questionCreated)
        {
            var question = questionCreated.Question;
            if (question.Length > 100)
            {
                question = question.Substring(0, 100);
            }
            Questions.Add(questionCreated.QuestionId, question);
        }
    }
}