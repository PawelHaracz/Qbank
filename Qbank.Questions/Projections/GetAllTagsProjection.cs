using System;
using System.Threading.Tasks;
using Qbank.Core.Projections;
using Qbank.Questions.Events.Tags;
using Qbank.Questions.Projections.States;

namespace Qbank.Questions.Projections
{
    public class GetAllTagsProjection : ProjectionBase<GetAllTagsProjection, TagNameState>
    {
        public override Guid Id => new Guid("6B5EDA6B-676B-4C68-8204-61E0B84B74AD");
        protected override void Map(IEventMapping mapping)
        {
            mapping.For<TagCreated>((meta, e)=> e.QuestionId.ToString(), Apply);
        }

        private Task Apply(Metadata metadata, TagCreated createdTag, TagNameState state)
        {
            state.Apply(createdTag);
            return Task.CompletedTask;
        }
    }
}