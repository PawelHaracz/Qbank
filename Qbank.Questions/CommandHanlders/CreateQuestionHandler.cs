using System;
using System.Threading.Tasks;
using Qbank.Core;
using Qbank.Core.Command;
using Qbank.Questions.Commands;
using Qbank.Questions.Events.Questions;
using Qbank.Questions.Events.Tags;

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
            var questionId = Guid.NewGuid();  

            var questionStreamId = $"{StreamPrefix.Question}_{command.CreatedOn}";                             

            await _eventStoreConnectionProvider.Execute<QuestionState>(questionStreamId, s => QuestionActions.Create(s, questionId, command.Question));
            await _eventStoreConnectionProvider.Execute<QuestionState>(questionStreamId, s => TagActions.Create(s, questionId, command.Tag));


            return questionId;
        }
    }
}