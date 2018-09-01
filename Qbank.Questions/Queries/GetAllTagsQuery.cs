using System;
using System.Collections.Generic;
using Qbank.Core.Queries;

namespace Qbank.Questions.Quries
{
    public class GetAllTagsQuery : IQuery<IDictionary<Guid, string>>
    {
        public string User { get; set; }
    }
}
