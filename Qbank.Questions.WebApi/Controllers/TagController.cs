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
    public class TagController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;
        private readonly ICommandDispatcher _commandDispatcher;

        public TagController(IQueryDispatcher queryDispatcher, ICommandDispatcher  commandDispatcher)
        {
            _queryDispatcher = queryDispatcher;
            _commandDispatcher = commandDispatcher;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IDictionary<Guid, string>))]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var query = new GetAllTagsQuery();
            var result = await _queryDispatcher.DispatchAsync(query).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPut("{name}/Question/{questionId}")]
        [ProducesResponseType(200, Type = typeof(Guid))]
        public async Task<ActionResult<Guid>> Post([FromRoute]string name, [FromRoute]Guid questionId)
        {
            var command = new CreateTag()
            {
                QuestionId = questionId,
                User = "PawelHaracz",
                TagName = name
            };
            var result = await _commandDispatcher.DispatchAsync(command).ConfigureAwait(false);
            return Ok(result);
        }
    }
}