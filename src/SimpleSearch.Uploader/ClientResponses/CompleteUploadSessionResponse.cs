using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleSearch.Uploader.ClientResponses
{
    public class CompleteUploadSessionResponse
    {
        public CompleteUploadSessionResponse()
        {
            CorruptedParts = Array.Empty<CorruptedPart>();
        }

        public CompleteUploadSessionResponse(IEnumerable<CorruptedPart> corruptedParts)
        {
            CorruptedParts = corruptedParts;
        }

        public IEnumerable<CorruptedPart> CorruptedParts { get; }
    }

    public class CorruptedPart
    {
        public string Id { get; set; }

        public long Offset { get; set; }

        public long ActualSizeInBytes { get; set; }

        public long ExpectedSizeInBytes { get; set; }

        public UploadState State => ActualSizeInBytes == 0 ? UploadState.NotUploaded : UploadState.Corrupted;

        [JsonConverter(typeof(StringEnumConverter))]
        public enum UploadState
        {
            Corrupted,
            NotUploaded
        }
    }
}