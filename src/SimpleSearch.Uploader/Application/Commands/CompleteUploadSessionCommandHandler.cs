using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using SimpleSearch.EventBus;
using SimpleSearch.Messages;
using SimpleSearch.Storage.Blobs;
using SimpleSearch.Storage.DocumentDb;
using SimpleSearch.Uploader.Application.Entities;
using SimpleSearch.Uploader.Application.Repositories;
using SimpleSearch.Uploader.ClientResponses;

namespace SimpleSearch.Uploader.Application.Commands
{
    public class CompleteUploadSessionCommandHandler : IRequestHandler<CompleteUploadSessionCommand, CompleteUploadSessionResponse>
    {
        private readonly IBlobStorage _blobStorage;
        private readonly ISessionsRepository _sessions;
        private readonly IEventBus _eventBus;

        public CompleteUploadSessionCommandHandler(IBlobStorage blobStorage,
            IEventBus eventBus, ISessionsRepository sessions)
        {
            _blobStorage = blobStorage;
            _eventBus = eventBus;
            _sessions = sessions;
        }

        public async Task<CompleteUploadSessionResponse> Handle(CompleteUploadSessionCommand request, CancellationToken cancellationToken)
        {
            var session = await _sessions.FindNotCompletedSessionAsync(request.UploadId, cancellationToken);
            if (session == null)
            {
                return null;
            }

            var uploadedBlockList = (await _blobStorage.GetBlockListAsync(session.Id, cancellationToken)).ToList();

            var corruptedParts = FindCorruptedParts(session, uploadedBlockList);
            if (corruptedParts.Count != 0)
            {
                return new CompleteUploadSessionResponse(corruptedParts);
            }

            await FinishUploadSessionAsync(session, uploadedBlockList, cancellationToken);

            _eventBus.Publish(
                new FileUploadedEvent
                {
                    UploadId = session.Id,
                    SizeInBytes = session.SizeInBytes,
                    FileName = session.FileName,
                    Extension = Enum.Parse<FileExtension>(session.Extension, true)
                });

            return new CompleteUploadSessionResponse();
        }

        private async Task FinishUploadSessionAsync(UploadSession session, List<BlockInfo> uploadedBlockList,
            CancellationToken cancellationToken)
        {
            await Task.WhenAll(
                _blobStorage.CommitBlockListAsync(session.Id, uploadedBlockList.Select(x => x.Id), cancellationToken),
                _sessions.CompleteSessionAsync(session.Id, cancellationToken));
        }

        private IList<CorruptedPart> FindCorruptedParts(UploadSession session, IEnumerable<BlockInfo> uploadedBlockList)
        {
            var corruptedParts = new List<CorruptedPart>();

            var uploadedBlocks = uploadedBlockList.ToDictionary(key => key.Id, val => val);
            foreach (var currentSessionPart in session.Parts)
            {
                if (!uploadedBlocks.TryGetValue(currentSessionPart.Id, out var uploadedBlock))
                {
                    corruptedParts.Add(new CorruptedPart
                    {
                        Id = currentSessionPart.Id,
                        Offset = currentSessionPart.Offset,
                        ExpectedSizeInBytes = currentSessionPart.SizeInBytes,
                        ActualSizeInBytes = 0
                    });
                }
                else
                {
                    if (uploadedBlock.SizeInBytes != currentSessionPart.SizeInBytes)
                    {
                        corruptedParts.Add(new CorruptedPart
                        {
                            Id = currentSessionPart.Id,
                            Offset = currentSessionPart.Offset,
                            ExpectedSizeInBytes = currentSessionPart.SizeInBytes,
                            ActualSizeInBytes = uploadedBlock.SizeInBytes
                        });
                    }
                }
            }

            return corruptedParts;
        }
    }
}