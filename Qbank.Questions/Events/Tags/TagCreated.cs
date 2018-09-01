using System;
using System.Runtime.Serialization;
using Qbank.Core.Event;

namespace Qbank.Questions.Events.Tags
{
    [DataContract]
    [EventTypeId(TypeId)]
    public class TagCreated : IEvent
    {
        public const string TypeId = "A03C5F84-7BBF-41A9-9105-5B01FBA6D33D";

        public TagCreated()
        {

        }

        public TagCreated(Guid questionId, string tagName)
        {
            TagName = tagName;
            QuestionId = questionId;
        }

        [DataMember(Order = 1)]
        public Guid QuestionId { get; set; }

        [DataMember(Order = 2)]
        public string TagName { get; set; }
        

        public override string ToString()
        {
            return $"{nameof(TagCreated)} with {nameof(TagName)} : {TagName}, {nameof(QuestionId)} : {QuestionId}";
        }
    }
}