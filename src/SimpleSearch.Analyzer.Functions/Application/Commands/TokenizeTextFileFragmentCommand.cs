using MediatR;
using SimpleSearch.Messages;

namespace SimpleSearch.Analyzer.Functions.Application.Commands
{
    public class TokenizeTextFileFragmentCommand : IRequest<FragmentTokenizedEvent>
    {
        public TokenizeTextFileFragmentCommand(string uploadId, string fileName, FileExtension extension, long offset,
            long length)
        {
            UploadId = uploadId;
            FileName = fileName;
            Offset = offset;
            Length = length;
            Extension = extension;
        }

        public string UploadId { get; }

        public string FileName { get; }

        public FileExtension Extension { get; }

        public long Offset { get; }

        public long Length { get; }
    }
}