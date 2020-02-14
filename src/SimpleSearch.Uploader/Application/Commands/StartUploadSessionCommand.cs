using System;
using MediatR;
using SimpleSearch.Uploader.ClientResponses;

namespace SimpleSearch.Uploader.Application.Commands
{
    public class StartUploadSessionCommand : IRequest<StartUploadSessionResponse>
    {
        public Guid Id { get; }

        public string FileName { get; }

        public long SizeInBytes { get; }

        public string Extension { get; }

        public StartUploadSessionCommand(string fileName, long sizeInBytes, string extension)
        {
            FileName = fileName;
            SizeInBytes = sizeInBytes;
            Extension = extension;
            Id = Guid.NewGuid();
        }
    }
}
