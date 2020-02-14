using MediatR;
using SimpleSearch.Uploader.ClientResponses;

namespace SimpleSearch.Uploader.Application.Commands
{
    public class CompleteUploadSessionCommand : IRequest<CompleteUploadSessionResponse>
    {
        public CompleteUploadSessionCommand(string uploadId)
        {
            UploadId = uploadId;
        }

        public string UploadId { get; }
    }
}