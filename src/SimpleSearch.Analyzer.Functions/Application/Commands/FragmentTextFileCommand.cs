using System.Collections.Generic;
using MediatR;
using SimpleSearch.Messages;

namespace SimpleSearch.Analyzer.Functions.Application.Commands
{
    public class FragmentTextFileCommand : IRequest<IEnumerable<FileFragmentedEvent>>
    {
        public FragmentTextFileCommand(string uploadId, long sizeInBytes, FileExtension extension, string fileName)
        {
            UploadId = uploadId;
            SizeInBytes = sizeInBytes;
            Extension = extension;
            FileName = fileName;
        }

        public string UploadId { get; }

        public FileExtension Extension { get; }

        public long SizeInBytes { get; }

        public string FileName { get; }
    }
}