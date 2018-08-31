using System;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Command;
using Qbank.Questions.Commands;
using Qbank.Questions.Events.Questions;

namespace Qbank.Questions.CommandHanlders
{
    public class CreateAnswerHandler : ICommandHandler<CreateAnswer>
    {
        private readonly IEventStoreConnectionProvider _eventStoreConnectionProvider;

        public CreateAnswerHandler(IEventStoreConnectionProvider eventStoreConnectionProvider)
        {
            _eventStoreConnectionProvider = eventStoreConnectionProvider;
        }

        public async Task<Guid> HandleAsync(CreateAnswer command)
        {
            var id = Guid.NewGuid();
            var questionStreamId = $"{StreamPrefix.Question}_{command.QuestionId}";
            await _eventStoreConnectionProvider.Execute<QuestionState>(questionStreamId, s => QuestionActions.Create(s, command.QuestionId, id, command.Answer, command.IsCorrect));

            return id;
        }
    }
}