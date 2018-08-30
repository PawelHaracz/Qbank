using System;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Command;
using Qbank.Questions.Commands;
using Qbank.Questions.Events.Tags;

namespace Qbank.Questions.CommandHanlders
{
    public class CreateTagHandler : ICommandHandler<CreateTag>
    {
        private readonly IEventStoreConnectionProvider _eventStoreConnectionProvider;

        public CreateTagHandler(IEventStoreConnectionProvider eventStoreConnectionProvider)
        {
            _eventStoreConnectionProvider = eventStoreConnectionProvider;
        }

        public async Task<Guid> HandleAsync(CreateTag command)
        {
            var tagId = Guid.NewGuid();
            var tagStreamId = $"{StreamPrefix.Tag}";
            var tagToQuesitonStreamId = $"{StreamPrefix.Tag}_{tagId}";

            var tasks = new[] {
                _eventStoreConnectionProvider.Execute<TagState>(tagStreamId, s => TagActions.Create(s, tagId, command.TagName)),
                _eventStoreConnectionProvider.Execute<TagState>(tagToQuesitonStreamId, s => TagActions.Create(s, tagId, command.TagName))
            };
            await Task.WhenAll(tasks);

            return tagId;
        }
    }
}