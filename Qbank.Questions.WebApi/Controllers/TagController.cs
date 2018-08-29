using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Qbank.Core.Queries;
using Qbank.Questions.QueryHandlers;
using Qbank.Questions.Quries;

namespace Qbank.Questions.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IQueryDispatcher _queryDispatcher;

        public TagController(IQueryDispatcher queryDispatcher)
        {
            _queryDispatcher = queryDispatcher;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IDictionary<Guid, string>))]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var query = new GetAllTagsQuery();
            var result = await _queryDispatcher.DispatchAsync(query).ConfigureAwait(false);
            return Ok(result);
        }
    }
}