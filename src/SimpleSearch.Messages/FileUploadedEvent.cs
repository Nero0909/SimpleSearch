using Newtonsoft.Json;
using SimpleSearch.Messages.Abstractions;

namespace SimpleSearch.Messages
{
    public class FileUploadedEvent : FileMessage
    {
        [JsonProperty]
        public long SizeInBytes { get; set; }
    }
}