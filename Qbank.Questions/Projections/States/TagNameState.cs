using System.Runtime.Serialization;
using Qbank.Questions.Events.Tags;

namespace Qbank.Questions.Projections.States
{
    [DataContract]
    public class TagNameState
    {
        [DataMember(Order = 1)]
        public string Name { get; set; }

        public void Apply(TagCreated questionCreated)
        {
            Name = questionCreated.TagName;
        }

        public override string ToString()
        {
            return $"{nameof(TagNameState)} - {nameof(Name)} : {Name}";
        }
    }
}