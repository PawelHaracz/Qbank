using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Qbank.Core.Command;
using Qbank.Core.Queries;
using Qbank.Questions.Commands;
using Qbank.Questions.Projections.States;
using Qbank.Questions.Quries;
using Qbank.Questions.WebApi.Models;

namespace Qbank.Questions.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        private readonly IQueryDispatcher _queryDispatcher;
        public QuestionsController(ICommandDispatcher commandDispatcher, IQueryDispatcher queryDispatcher)
        {
            _commandDispatcher = commandDispatcher;
            _queryDispatcher = queryDispatcher;
        }
        // GET api/questions
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IDictionary<Guid, string>))]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var query = new GetAllUserQuestionsQuery()
            {
                User = "PawelHaracz"
            };
            var result = await _queryDispatcher.DispatchAsync(query).ConfigureAwait(false);
            return Ok(result);
        }

        // GET api/questions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionWithAnswersState>> Get(Guid id)
        {
            var getQuestionWithAnswersQuery = new GetQuestionWithAnswersQuery()
            {
                QuestionId = id
            };
            var result = await _queryDispatcher.DispatchAsync(getQuestionWithAnswersQuery).ConfigureAwait(false);
            return result;
        }

        // POST api/questions/{tagName}
        [HttpPost("{tagName}")]
        [ProducesResponseType(200, Type = typeof(Guid))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]QustionWithAnserwsDto question, [FromRoute]string tagName)
        {

            if (question == null)
            {
                return BadRequest(new ArgumentNullException($"{nameof(question)}"));
            }
            if (string.IsNullOrWhiteSpace(question.Question))
            {
                return BadRequest(new ArgumentNullException($"{nameof(question.Question)}"));
            }

            if (string.IsNullOrWhiteSpace(tagName))
            {
                return BadRequest(new ArgumentNullException($"{nameof(tagName)}"));
            }

            var isAnyAnswer = question.Answers != null && question.Answers.Any();
            if (isAnyAnswer && question.Answers.Any(a => a.IsCorrect) == false)
            {
                return BadRequest(new ArgumentException("Any answer isn't correct", $"{nameof(question.Answers)}"));
            }

            var command = new CreateQuestion()
            {
                Question = question.Question,
                CreatedOn = "PawelHaracz",
                Tag = tagName
            };

            var questionId = await _commandDispatcher.DispatchAsync(command).ConfigureAwait(false);


            //implement service bus for that
            if (isAnyAnswer)
            {
                var aneswerCommands = question.Answers.Select(a => new CreateAnswer()
                {
                    QuestionId = questionId,
                    Answer = a.Answer,
                    IsCorrect = a.IsCorrect,
                    CreatedOn = command.CreatedOn
                });
                var answerTasks = aneswerCommands.Select(answer => _commandDispatcher.DispatchAsync(answer)).Cast<Task>().ToList();

                await Task.WhenAll(answerTasks).ConfigureAwait(false);
            }

            return Ok(questionId);
        }
    }
}
