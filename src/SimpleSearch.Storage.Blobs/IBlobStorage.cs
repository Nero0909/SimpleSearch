using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleSearch.Storage.Blobs
{
    public interface IBlobStorage
    {
        Task<bool> UploadBlockBlob(byte[] data, string containerName, string blockId,
            CancellationToken cancellationToken);

        Task<IEnumerable<BlockInfo>> GetBlockListAsync(string containerName, CancellationToken cancellationToken);

        Task<bool> CommitBlockListAsync(string containerName, IEnumerable<string> blockIds,
            CancellationToken cancellationToken);

        Task<bool> ReadRangeToStreamAsync(string containerName, Stream stream, long offset, long length,
            CancellationToken cancellationToken);
    }
}
