using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SimpleSearch.Analyzer.Functions.Application.Commands;
using SimpleSearch.Analyzer.Functions.Application.Infrastructure;
using SimpleSearch.Messages;

namespace SimpleSearch.Analyzer.Functions.Functions
{
    public class TokenizeFunctions
    {
        private readonly IMediator _mediator;

        public TokenizeFunctions(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task TokenizeFragment(
            [RabbitMQTrigger(QueueName.Tokenizer, ConnectionStringSetting = "RabbitMQ:ConnectionString")]
            FileFragmentedEvent message,
            [RabbitMQ(QueueName = QueueName.Indexer, ConnectionStringSetting = "RabbitMQ:ConnectionString")]
            IAsyncCollector<FragmentTokenizedEvent> rabbitCollector,
            ILogger logger)
        {
            logger.LogInformation($"Start tokenization for: {message.UploadId}");

            var command = GenerateCommand(message);
            var output = await _mediator.Send(command);
            if (output != null)
            {
                await rabbitCollector.AddAsync(output);
            }
        }

        private IRequest<FragmentTokenizedEvent> GenerateCommand(FileFragmentedEvent message) =>
            message.Extension switch
            {
                FileExtension.Txt => new TokenizeTextFileFragmentCommand(
                    message.UploadId, message.FileName, message.Extension,
                    message.Offset, message.Length),
                _ => throw new ArgumentOutOfRangeException(nameof(message.Extension), message.Extension, null)
            };
    }
}