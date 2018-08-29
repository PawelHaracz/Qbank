using System;
using System.Runtime.Serialization;
using Qbank.Core.Event;

namespace Qbank.Questions.Events.Tags
{
    [DataContract]
    [EventTypeId(TypeId)]
    public class AssosiatedQuestionToTag : IEvent
    {
        public const string TypeId = "E9B73A26-A5E9-49EF-897C-6DC7E184D6D4";

        public AssosiatedQuestionToTag()
        {

        }

        public AssosiatedQuestionToTag(Guid tagId, Guid questionId)
        {
            TagId = tagId;
            QuestionId = questionId;
        }

        [DataMember(Order = 1)]
        public Guid TagId { get; }

        [DataMember(Order = 2)]
        public Guid QuestionId { get; }

        public override string ToString()
        {
            return $"{nameof(AssosiatedQuestionToTag)} with {nameof(TagId)} : {TagId}, {nameof(QuestionId)} : {QuestionId}";
        }
    }
}