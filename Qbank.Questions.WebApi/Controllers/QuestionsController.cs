using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Qbank.Core.Command;
using Qbank.Questions.Commands;

namespace Qbank.Questions.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly ICommandDispatcher _dispatcher;

        public QuestionsController(ICommandDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        // GET api/questions
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/questions/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
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
           var id = await _dispatcher.DispatchAsync(command).ConfigureAwait(false);
            return Ok(id);
        }

          
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
