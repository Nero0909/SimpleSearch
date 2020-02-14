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

namespace SimpleSearch.Uploader.Application.Commands
{
    public class UploadPartCommandHandler : IRequestHandler<UploadPartCommand, bool>
    {
        private readonly IBlobStorage _blobStorage;
        private readonly IMongoDbContext<UploadSession> _context;

        public UploadPartCommandHandler(IBlobStorage blobStorage, IMongoDbContext<UploadSession> context)
        {
            _blobStorage = blobStorage;
            _context = context;
        }

        public async Task<bool> Handle(UploadPartCommand request, CancellationToken cancellationToken)
        {
            var session = (await _context.Collection.FindAsync(
                Builders<UploadSession>.Filter.Where(s => s.Id == request.UploadId && !s.IsCompleted),
                cancellationToken: cancellationToken)).FirstOrDefault();

            if (session == null)
            {
                return false;
            }

            return await _blobStorage.UploadBlockBlob(request.Part, request.UploadId, request.PartId, cancellationToken);
        }
    }
}