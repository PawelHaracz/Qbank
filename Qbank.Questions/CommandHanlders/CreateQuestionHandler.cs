using System;
using System.Threading.Tasks;
using Qbank.Core.Command;
using Qbank.Core.Event;
using Qbank.Questions.Commands;
using Qbank.Questions.Events;
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
            var tagId = Guid.NewGuid();

            var questionStreamId = $"Question_{DateTime.Now:yy-MM-dd:HH-mm-ss}_{command.CreatedOn}";
            var tagStreamId = $"Tag";
            var tagToQuesitonStreamId = $"Tag_{tagId}";
            //wrap it in transaction
            var questionTask = _eventStoreConnectionProvider.Execute<QuestionState>(questionStreamId, s => QuestionActions.Create(s, questionId, command.Question));
            var tagGlobalTask = _eventStoreConnectionProvider.Execute<TagState>(tagStreamId, s => TagActions.Create(s, tagId, command.Tag));
            var tagTask = _eventStoreConnectionProvider.Execute<TagState>(tagToQuesitonStreamId, s => TagActions.Create(s, tagId, command.Tag));
            var assosiateQuestionToTagTask = _eventStoreConnectionProvider.Execute<TagState>(tagToQuesitonStreamId, s => TagActions.Assosiate(s, tagId, questionId));

            await Task.WhenAll(questionTask, tagGlobalTask, tagTask, assosiateQuestionToTagTask).ConfigureAwait(false);

            return questionId;
        }
    }
}