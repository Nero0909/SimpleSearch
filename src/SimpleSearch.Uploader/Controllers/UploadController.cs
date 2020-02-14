using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleSearch.Uploader.Application.Commands;
using SimpleSearch.Uploader.ClientRequests;

namespace SimpleSearch.Uploader.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("uploads")]
        public async Task<IActionResult> InitializeUploadAsync([FromBody] StartUploadSessionModel model,
            CancellationToken cancellationToken)
        {
            var command = new StartUploadSessionCommand(model.FileName, model.SizeInBytes, model.Extension.ToString());
            var entity = await _mediator.Send(command, cancellationToken);

            return Ok(entity);
        }

        [HttpPut("uploads/{uploadid}/parts/{partid}")]
        public async Task<IActionResult> UploadPartAsync([FromRoute] string uploadId, [FromRoute] string partId,
            CancellationToken cancellationToken)
        {
            await using var ms = new MemoryStream((int?) Request.ContentLength ?? 2048);
            await Request.Body.CopyToAsync(ms, cancellationToken);

            var command = new UploadPartCommand(ms.ToArray(), partId, uploadId);
            var success = await _mediator.Send(command, cancellationToken);

            if (success)
            {
                return Ok();
            }

            return BadRequest();
        }

        [HttpPut("uploads/{uploadid}")]
        public async Task<IActionResult> CompleteUploadSession([FromRoute] string uploadId,
            CancellationToken cancellationToken)
        {
            var command = new CompleteUploadSessionCommand(uploadId);
            var result = await _mediator.Send(command, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            if (result.CorruptedParts.Any())
            {
                return BadRequest(result);
            }

            return Ok();
        }
    }
}