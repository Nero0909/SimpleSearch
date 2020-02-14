using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using SimpleSearch.Storage.Blobs;
using SimpleSearch.Storage.DocumentDb;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Repositories;

namespace SimpleSearch.Uploader.Application.Commands
{
    public class UploadPartCommandHandler : IRequestHandler<UploadPartCommand, bool>
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ISessionsRepository _sessions;

        public UploadPartCommandHandler(IBlobStorage blobStorage, ISessionsRepository sessions)
        {
            _blobStorage = blobStorage;
            _sessions = sessions;
        }

        public async Task<bool> Handle(UploadPartCommand request, CancellationToken cancellationToken)
        {
            var session = _sessions.FindNotCompletedSessionAsync(request.UploadId, cancellationToken);

            if (session == null)
            {
                return false;
            }

            return await _blobStorage.UploadBlockBlob(request.Part, request.UploadId, request.PartId, cancellationToken);
        }
    }
}