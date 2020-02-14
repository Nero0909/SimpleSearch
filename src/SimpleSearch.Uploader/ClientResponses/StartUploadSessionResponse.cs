using System;
using System.Collections.Generic;
using SimpleSearch.Uploader.Application.Entities;

namespace SimpleSearch.Uploader.ClientResponses
{
    public class StartUploadSessionResponse
    {
        public string Id { get; set; }

        public long SizeInBytes { get; set; }

        public string FileName { get; set; }

        public string Extension { get; set; }

        public IEnumerable<UploadPart> Parts { get; set; } = Array.Empty<UploadPart>();
    }
}
