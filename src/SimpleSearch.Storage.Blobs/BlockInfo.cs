using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleSearch.Storage.Blobs
{
    public class BlockInfo
    {
        public BlockInfo(string id, long sizeInBytes)
        {
            Id = id;
            SizeInBytes = sizeInBytes;
        }

        public string Id { get; }

        public long SizeInBytes { get; }
    }
}
