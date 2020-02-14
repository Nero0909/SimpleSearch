using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SimpleSearch.Uploader.ClientResponses
{
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