using System;
using System.Threading.Tasks;
using Qbank.Core.Command;
using Qbank.Core.Event;
using Qbank.Questions.Commands;

namespace Qbank.Questions.CommandHanlders
{
    public class CreateQuestionHandler : ICommandHandler<CreateQuestion>
    {
        private readonly IEventStoreConnectionProvider _eventStoreConnectionProvider;
        public CreateQuestionHandler(IEventStoreConnectionProvider eventStoreConnectionProvider)
        {
            _eventStoreConnectionProvider = eventStoreConnectionProvider;
        }

        public async Task<Guid> HandleAsync(CreateQuestion command)
        {
            var id = Guid.NewGuid();
            var streamId = $"Question_{DateTime.Now.ToString("yy-MM-dd:HH-mm-ss")}_{command.CreatedOn}_{id}";
            await _eventStoreConnectionProvider.Execute<QuestionState>(streamId, s => QuestionActions.Create(s,id, command.Question)).ConfigureAwait(false);

            return id;
        }
    }
}