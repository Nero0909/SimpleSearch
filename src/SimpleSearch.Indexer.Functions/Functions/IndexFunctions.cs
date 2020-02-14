using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SimpleSearch.Indexer.Functions.Application.Commands;
using SimpleSearch.Messages;

namespace SimpleSearch.Indexer.Functions.Functions
{
    public class IndexFunctions
    {
        private readonly IMediator _mediator;

        public IndexFunctions(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task IndexFragment(
            [RabbitMQTrigger(QueueName.Indexer, ConnectionStringSetting = "RabbitMQ:ConnectionString")]
            FragmentTokenizedEvent message,
            ILogger logger)
        {
            logger.LogInformation($"Start indexing for: {message.UploadId}");

            var command = new IndexFragmentCommand(message.UploadId, message.FileName, message.Extension,
                message.Offset, message.Length,
                message.Tokens);

            var output = await _mediator.Send(command);
        }
    }
}