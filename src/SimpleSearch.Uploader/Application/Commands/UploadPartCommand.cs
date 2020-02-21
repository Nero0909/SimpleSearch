using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleSearch.Uploader.Application.Commands
{
    public class UploadPartCommand : IRequest<bool>
    {
        public Stream Part { get; }

        public string PartId { get; }

        public string UploadId { get; }

        public UploadPartCommand(Stream part, string partId, string uploadId)
        {
            Part = part;
            PartId = partId;
            UploadId = uploadId;
        }
    }
}
