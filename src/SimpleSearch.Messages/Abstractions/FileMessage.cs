using Newtonsoft.Json;

namespace SimpleSearch.Messages.Abstractions
{
    public abstract class FileMessage : BaseMessage
    {
        [JsonProperty]
        public string UploadId { get; set; }

        [JsonProperty]
        public string FileName { get; set; }

        [JsonProperty]
        public FileExtension Extension { get; set; }
    }
}