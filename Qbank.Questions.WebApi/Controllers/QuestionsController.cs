using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Qbank.Core.Command;
using Qbank.Core.Queries;
using Qbank.Questions.Commands;
using Qbank.Questions.Quries;

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
            var query = new GetAllUserQuestions()
            {
                User = "PawelHaracz"
            };
            var result = await _queryDispatcher.DispatchAsync(query).ConfigureAwait(false);
            return Ok(result);
        }

        // GET api/questions/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(Guid id)
        {
            return "command";
        }

        // POST api/questions
        [HttpPost]
        [ProducesResponseType(200, Type = typeof(Guid))]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]string question)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                return BadRequest($"{nameof(question)} is null or empty");
            }
            var command = new CreateQuestion()
            {
                Question = question,
                CreatedOn = "PawelHaracz",
                Tag = "Test"
            };
           var id = await _commandDispatcher.DispatchAsync(command).ConfigureAwait(false);
            return Ok(id);
        }

          
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
