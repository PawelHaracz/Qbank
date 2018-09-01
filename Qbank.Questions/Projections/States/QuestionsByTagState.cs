using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Qbank.Questions.Events.Tags;

namespace Qbank.Questions.Projections.States
{
    [DataContract]
    public class QuestionsByTagState
    {
        [DataMember(Order = 1)]
        public HashSet<Guid> Questions => new HashSet<Guid>();

        public void Apply(TagCreated tagCreated)
        {
            Questions.Add(tagCreated.QuestionId);
        }
    }
}