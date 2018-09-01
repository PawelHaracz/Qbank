using System;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Command;
using Qbank.Questions.Commands;
using Qbank.Questions.Events.Questions;
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
            var streamId = $"{StreamPrefix.Question}_{command.User}";
            await _eventStoreConnectionProvider.Execute<QuestionState>(streamId, s => TagActions.Create(s, command.QuestionId, command.TagName));
 
            return command.QuestionId;
        }
    }
}