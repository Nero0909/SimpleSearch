using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleSearch.Indexer.Application;
using SimpleSearch.Indexer.ClientRequests;

namespace SimpleSearch.Indexer.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IQueryParser _queryParser;

        public SearchController(IMediator mediator, IQueryParser queryParser)
        {
            _mediator = mediator;
            _queryParser = queryParser;
        }

        [HttpPost("search")]
        public async Task<IActionResult> InitializeUploadAsync([FromBody] SearchRequest request,
            CancellationToken cancellationToken)
        {
            var query = _queryParser.ParseQuery(request);
            if (query == null)
            {
                return BadRequest("Invalid query");
            }

            var response = await _mediator.Send(query, cancellationToken);

            return Ok(response);
        }
    }
}