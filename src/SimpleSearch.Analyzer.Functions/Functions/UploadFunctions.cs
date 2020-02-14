using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SimpleSearch.Analyzer.Functions.Application.Commands;
using SimpleSearch.Analyzer.Functions.Application.Infrastructure;
using SimpleSearch.Messages;

namespace SimpleSearch.Analyzer.Functions.Functions
{
    public class UploadFunctions
    {
        private readonly IMediator _mediator;

        public UploadFunctions(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task FragmentUploadedFile(
            [RabbitMQTrigger(SubscriptionName.Analyzer, ConnectionStringSetting = "RabbitMQ:ConnectionString")]
            FileUploadedEvent message,
            [RabbitMQ(QueueName = QueueName.Tokenizer, ConnectionStringSetting = "RabbitMQ:ConnectionString")]
            IAsyncCollector<FileFragmentedEvent> rabbitCollector,
            ILogger logger)
        {
            logger.LogInformation($"Start fragmenting message: {message.UploadId}");

            var command = GenerateCommand(message);
            var messages = await _mediator.Send(command);

            await Task.WhenAll(messages.Select(m => rabbitCollector.AddAsync(m)));
        }

        private IRequest<IEnumerable<FileFragmentedEvent>> GenerateCommand(FileUploadedEvent message) =>
            message.Extension switch
            {
                FileExtension.Txt => new FragmentTextFileCommand(
                    message.UploadId, message.SizeInBytes,
                    message.Extension, message.FileName),
                _ => throw new ArgumentOutOfRangeException(nameof(message.Extension), message.Extension, null)
            };
    }
}