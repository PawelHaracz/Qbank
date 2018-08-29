using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Qbank.Questions.Events.Questions;

namespace Qbank.Questions.Projections.Models
{
    [DataContract]
    public class QuestionTeasersWith100Characters
    {  
        [DataMember(Order = 1)]
        public Dictionary<Guid, string> Questions { get; set; } = new Dictionary<Guid, string>();  

        
        public void Apply(QuestionCreated questionCreated)
        {
            Questions.Add(questionCreated.QuestionId, questionCreated.Question.Substring(0,100));
        }
    }
}