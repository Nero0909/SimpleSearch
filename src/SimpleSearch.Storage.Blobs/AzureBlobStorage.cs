using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace SimpleSearch.Storage.Blobs
{
    public class AzureBlobStorage : IBlobStorage
    {
        private readonly CloudBlobContainer _cloudBlobContainer;
        private const string BlobNotFound = "BlobNotFound";

        public AzureBlobStorage(IOptions<BlobSettings> settings)
        {
            var blobSettings = settings.Value;
            _cloudBlobContainer = CloudStorageAccount
                .Parse(blobSettings.ConnectionString)
                .CreateCloudBlobClient()
                .GetContainerReference(blobSettings.ContainerName);

            _cloudBlobContainer.CreateIfNotExists();
        }

        public async Task<bool> UploadBlockBlob(Stream data, string containerName, string blockId, CancellationToken cancellationToken)
        {
            try
            {
                var checkSum = await GetMd5HashFromStreamAsync(data);
                await _cloudBlobContainer.GetBlockBlobReference(containerName)
                    .PutBlockAsync(blockId, data, checkSum, cancellationToken);
                return true;
            }
            catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobNotFound)
            {
                return false;
            }
        }

        public async Task<IEnumerable<BlockInfo>> GetBlockListAsync(string containerName, CancellationToken cancellationToken)
        {
            try
            {
                var blockList = await _cloudBlobContainer.GetBlockBlobReference(containerName)
                    .DownloadBlockListAsync(BlockListingFilter.All, null, null, null, cancellationToken);

                return blockList.Select(x => new BlockInfo(x.Name, x.Length));
            }
            catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobNotFound)
            {
                return Enumerable.Empty<BlockInfo>();
            }
        }

        public async Task<bool> CommitBlockListAsync(string containerName, IEnumerable<string> blockIds, CancellationToken cancellationToken)
        {
            try
            {
                var block = _cloudBlobContainer.GetBlockBlobReference(containerName);
                await block.PutBlockListAsync(blockIds, cancellationToken);
                return true;
            }
            catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobNotFound)
            {
                return false;
            }
        }

        public async Task<bool> ReadRangeToStreamAsync(string containerName, Stream stream, long offset, long length, CancellationToken cancellationToken)
        {
            try
            {
                var block = _cloudBlobContainer.GetBlockBlobReference(containerName);
                await block.DownloadRangeToStreamAsync(stream, offset, length, cancellationToken);
                return true;
            }
            catch (StorageException e) when (e.RequestInformation.ErrorCode == BlobNotFound)
            {
                return false;
            }
        }

        private async Task<string> GetMd5HashFromStreamAsync(Stream stream)
        {
            using var md5Check = MD5.Create();

            var buffer = new byte[1024];
            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer, CancellationToken.None);
                if (bytesRead == 0)
                {
                    break;
                }

                md5Check.TransformBlock(buffer, 0, buffer.Length, null, 0);
            }

            stream.Position = 0;

            md5Check.TransformFinalBlock(new byte[0], 0, 0);

            var hashBytes = md5Check.Hash;
            return Convert.ToBase64String(hashBytes);
        }
    }
}