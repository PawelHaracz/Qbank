using System;
using System.Runtime.Serialization;
using Qbank.Core.Event;

namespace Qbank.Questions.Events
{
    [DataContract]
    [EventTypeId(TypeId)]
    public class AnswerCreated : IEvent
    {
        public const string TypeId = "A97EEEB4-9BD8-42DE-BD9C-CC86A689B73B";

        AnswerCreated()
        {
                
        }

        public AnswerCreated(Guid answerId, Guid questionId, string answer, bool isCorrect)
        {
            AnswerId = answerId;
            QuestionId = questionId;
            Answer = answer;
            IsCorrect = isCorrect;
        }
        [DataMember(Order = 1)]
        public Guid AnswerId { get; set; }

        [DataMember(Order = 2)]
        public Guid QuestionId { get; set; }

        [DataMember(Order = 3)]
        public string Answer { get; set; }

        [DataMember(Order = 4)]
        public bool IsCorrect { get; set; }

        public override string ToString()
        {
            return $"{nameof(AnswerCreated)} with id : {AnswerId}, related to question {QuestionId}";
        }
    }
}