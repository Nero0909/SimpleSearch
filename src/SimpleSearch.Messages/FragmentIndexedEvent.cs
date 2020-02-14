using Newtonsoft.Json;
using SimpleSearch.Messages.Abstractions;

namespace SimpleSearch.Messages
{
    public class FragmentIndexedEvent : FileMessage
    {
        [JsonProperty]
        public long Offset { get; set; }

        [JsonProperty]
        public long Length { get; set; }
    }
}