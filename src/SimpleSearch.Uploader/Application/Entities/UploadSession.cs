using System;
using System.Collections.Generic;
using SimpleSearch.Storage.DocumentDb;

namespace SimpleSearch.Uploader.Application.Entities
{
    public class UploadSession : DbEntity
    {
        public long SizeInBytes { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public bool IsCompleted { get; set; }

        public IEnumerable<UploadPart> Parts { get; set; } = Array.Empty<UploadPart>();
    }
}
