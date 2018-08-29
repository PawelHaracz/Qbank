using System;
using System.Runtime.Serialization;
using Qbank.Core.Event;

namespace Qbank.Questions.Events.Tags
{
    [DataContract]
    [EventTypeId(TypeId)]
    public class CreatedTag : IEvent
    {
        public const string TypeId = "A03C5F84-7BBF-41A9-9105-5B01FBA6D33D";

        public CreatedTag()
        {

        }

        public CreatedTag(Guid tagId, string tagName)
        {
            TagId = tagId;
            TagName = tagName;
        }

        [DataMember(Order = 1)]
        public Guid TagId { get; set; }

        [DataMember(Order = 2)]
        public string TagName { get; set; }

        public override string ToString()
        {
            return $"{nameof(CreatedTag)} with {nameof(TagId)} : {TagId}, {nameof(TagName)} : {TagName}";
        }
    }
}