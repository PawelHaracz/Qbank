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
        public IList<Guid> Questions => new List<Guid>();

        public void Apply(AssosiatedQuestionToTag assosiatedQuestionToTag)
        {
            if (Questions.Contains(assosiatedQuestionToTag.QuestionId))
            {
                return;
            }
            Questions.Add(assosiatedQuestionToTag.TagId);
        }
    }
}