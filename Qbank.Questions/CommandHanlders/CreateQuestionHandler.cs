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
            var tagId = Guid.NewGuid();

            var questionStreamId = $"{StreamPrefix.Question}_{command.CreatedOn}";
            var questionDetail = $"{StreamPrefix.Question}_{questionId}";
            var tagStreamId = $"{StreamPrefix.Tag}";
            var tagToQuesitonStreamId = $"{StreamPrefix.Tag}_{tagId}";
                     

            var questionTask = _eventStoreConnectionProvider.Execute<QuestionState>(questionStreamId, s => QuestionActions.Create(s, questionId, command.Question));
            var questionDetailTask = _eventStoreConnectionProvider.Execute<QuestionState>(questionDetail, s => QuestionActions.Create(s, questionId, command.Question)); //isn't work !! the same state (remove glo
            //move to another commandHandler
            var tagGlobalTask = _eventStoreConnectionProvider.Execute<TagState>(tagStreamId, s => TagActions.Create(s, tagId, command.Tag));
            var tagTask = _eventStoreConnectionProvider.Execute<TagState>(tagToQuesitonStreamId, s => TagActions.Create(s, tagId, command.Tag)); //isn't work !! the same state
            var assosiateQuestionToTagTask = _eventStoreConnectionProvider.Execute<TagState>(tagToQuesitonStreamId, s => TagActions.Assosiate(s, tagId, questionId));

            await Task.WhenAll(questionTask, questionDetailTask, tagGlobalTask, tagTask).ConfigureAwait(false);
            await assosiateQuestionToTagTask.ConfigureAwait(false);

            return questionId;
        }
    }
}