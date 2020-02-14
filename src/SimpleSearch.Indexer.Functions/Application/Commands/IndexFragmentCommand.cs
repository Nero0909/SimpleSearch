using System;
using System.Collections.Generic;
using MediatR;
using SimpleSearch.Messages;

namespace SimpleSearch.Indexer.Functions.Application.Commands
{
    public class IndexFragmentCommand : IRequest<FragmentIndexedEvent>
    {
        public IndexFragmentCommand(string uploadId, string fileName, FileExtension extension, long offset, long length,
            IEnumerable<Token> tokens)
        {
            UploadId = uploadId;
            FileName = fileName;
            Offset = offset;
            Length = length;
            Extension = extension;
            Tokens = tokens ?? Array.Empty<Token>();
        }

        public string UploadId { get; }

        public string FileName { get; }

        public FileExtension Extension { get; }

        public long Offset { get; }

        public long Length { get; }

        public IEnumerable<Token> Tokens { get; }
    }
}