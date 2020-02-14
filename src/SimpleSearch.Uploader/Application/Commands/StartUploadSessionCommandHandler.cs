using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SimpleSearch.Storage.DocumentDb;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Settings;
using DistributionStrategy = SimpleSearch.Uploader.Application.Services.DistributionStrategy;

namespace SimpleSearch.Uploader.Application.Commands
{
    public class StartUploadSessionCommandHandler : IRequestHandler<StartUploadSessionCommand, UploadSession>
    {
        private readonly IMongoDbContext<UploadSession> _context;
        private readonly UploadSettings _settings;

        public StartUploadSessionCommandHandler(IMongoDbContext<UploadSession> context,
            IOptions<UploadSettings> settings)
        {
            _settings = settings.Value;
            _context = context;
        }

        public async Task<UploadSession> Handle(StartUploadSessionCommand request, CancellationToken cancellationToken)
        {
            var sessionId = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            var entity = new UploadSession
            {
                Id = sessionId,
                FileName = request.FileName,
                SizeInBytes = request.SizeInBytes,
                Parts = GenerateUploadParts(request.SizeInBytes),
                Extension = request.Extension
            };

            await _context.Collection.InsertOneAsync(entity, new InsertOneOptions(), cancellationToken);

            return entity;
        }

        private IEnumerable<UploadPart> GenerateUploadParts(long totalSizeInBytes)
        {
            var sizeDistribution = DistributionStrategy.BySize(totalSizeInBytes, _settings.ChunkSizeInBytes);

            var parts = new UploadPart[sizeDistribution.Count];
            var offset = 0L;
            for (var i = 0; i < parts.Length; i++)
            {
                parts[i] = new UploadPart
                {
                    Id = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{i:0000000}")),
                    Offset = offset,
                    SizeInBytes = sizeDistribution[i]
                };
                offset += sizeDistribution[i];
            }

            return parts;
        }
    }
}
