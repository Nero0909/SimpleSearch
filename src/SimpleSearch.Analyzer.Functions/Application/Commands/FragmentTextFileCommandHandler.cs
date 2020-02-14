using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using SimpleSearch.Analyzer.Functions.Application.Extensions;
using SimpleSearch.Analyzer.Functions.Application.Settings;
using SimpleSearch.Messages;
using SimpleSearch.Storage.Blobs;

namespace SimpleSearch.Analyzer.Functions.Application.Commands
{
    public class FragmentTextFileCommandHandler : IRequestHandler<FragmentTextFileCommand, IEnumerable<FileFragmentedEvent>>
    {
        private readonly IBlobStorage _blobStorage;
        private readonly FragmentationSettings _settings;
        private const int MaxBytesToRead = 255;
        private const int FailureFlag = -1;

        public FragmentTextFileCommandHandler(IBlobStorage blobStorage, IOptions<FragmentationSettings> options)
        {
            _blobStorage = blobStorage;
            _settings = options.Value;
        }

        public async Task<IEnumerable<FileFragmentedEvent>> Handle(FragmentTextFileCommand request, CancellationToken cancellationToken)
        {
            if (request.SizeInBytes <= _settings.ChunkSizeInBytes)
            {
                return new[]
                {
                    new FileFragmentedEvent
                    {
                        Extension = request.Extension,
                        UploadId = request.UploadId,
                        FileName = request.FileName,
                        Offset = 0,
                        Length = request.SizeInBytes
                    }
                };
            }

            var initialOffsets = FindInitialOffsets(request.SizeInBytes, _settings.ChunkSizeInBytes);

            var rightBorders = await Task.WhenAll(initialOffsets.Select(offset =>
                FindRightBorderAsync(offset, request.SizeInBytes, request.UploadId, cancellationToken)));

            return ConstructEvents(rightBorders, request);
        }

        private async Task<long> FindRightBorderAsync(long offset, long fileSize, string uploadId, CancellationToken cancellationToken)
        {
            if (offset == fileSize)
            {
                return offset;
            }

            var bytesToRead = Math.Min(fileSize - offset, MaxBytesToRead);

            await using var ms = new MemoryStream();
            var successRead = await _blobStorage.ReadRangeToStreamAsync(uploadId, ms, offset, bytesToRead, cancellationToken);
            if (!successRead)
            {
                return FailureFlag;
            }

            ms.Position = 0;
            using var sr = new StreamReader(ms);

            var readSoFar = 0;
            while (!sr.EndOfStream)
            {
                var ch = (char)sr.Read();
                
                if (ch.IsSeparator())
                {
                    return offset + readSoFar;
                }
                readSoFar++;
            }

            return FailureFlag;
        }

        private IEnumerable<long> FindInitialOffsets(long fileSize, long chunkSize)
        {
            var current = chunkSize;

            while (current < fileSize)
            {
                yield return current;
                current += chunkSize;
            }
        }

        private IEnumerable<FileFragmentedEvent> ConstructEvents(long[] rightBorders, FragmentTextFileCommand request)
        {
            var left = 0L;
            foreach (var right in rightBorders.Where(b => b != FailureFlag).OrderBy(x => x))
            {
                yield return new FileFragmentedEvent
                {
                    Extension = request.Extension,
                    UploadId = request.UploadId,
                    FileName = request.FileName,
                    Offset = left,
                    Length = right - left
                };

                left = right + 1;
            }

            // The last part
            yield return new FileFragmentedEvent
            {
                Extension = request.Extension,
                UploadId = request.UploadId,
                FileName = request.FileName,
                Offset = left,
                Length = request.SizeInBytes - left
            };
        }
    }
}