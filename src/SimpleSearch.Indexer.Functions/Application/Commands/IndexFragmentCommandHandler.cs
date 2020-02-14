using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using SimpleSearch.Indexer.Functions.Application.Extensions;
using SimpleSearch.Indexer.Shared;
using SimpleSearch.Indexer.Shared.Entities;
using SimpleSearch.Messages;

namespace SimpleSearch.Indexer.Functions.Application.Commands
{
    public class IndexFragmentCommandHandler : IRequestHandler<IndexFragmentCommand, FragmentIndexedEvent>
    {
        private readonly ITokensRepository _tokens;

        public IndexFragmentCommandHandler(ITokensRepository tokens)
        {
            _tokens = tokens;
        }

        public async Task<FragmentIndexedEvent> Handle(IndexFragmentCommand request, CancellationToken cancellationToken)
        {
            await request.Tokens.Select(token => AddDocumentToToken(token, request, cancellationToken))
                .Throttle(Environment.ProcessorCount);

            return new FragmentIndexedEvent
            {
                FileName = request.FileName,
                UploadId = request.UploadId,
                Extension = request.Extension,
                Offset = request.Offset,
                Length = request.Length
            };
        }

        private Task AddDocumentToToken(Token token, IndexFragmentCommand command,
            CancellationToken cancellationToken)
        {
            var documentIndex = new DocumentEntity
                {Id = command.UploadId, Name = command.FileName, Extension = command.Extension.ToString()};

            return _tokens.AddDocumentToTokenAsync(documentIndex, token.Tag, cancellationToken);
        }
    }
}