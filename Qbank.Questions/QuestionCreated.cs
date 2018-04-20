using System;
using System.Runtime.Serialization;
using Qbank.Core;

namespace Qbank.Questions
{
    [DataContract]
    [EventTypeId(TypeId)]
    public class QuestionCreated : IEvent
    {
        public const string TypeId = "186E242F-6D5C-465E-9318-05B7AE0F2A3A";

        QuestionCreated()
        {
            
        }

        public QuestionCreated(Guid questionId,string question)
        {
            QuestionId = questionId;
            Question = question;
        }

        [DataMember(Order = 1)]
        public Guid QuestionId { get; set; }
        [DataMember(Order = 2)]
        public string Question { get; set; }

        public override string ToString()
        {
            return $"{nameof(QuestionCreated)} with id: {QuestionId} question: {Question}";
        }
    }
}
