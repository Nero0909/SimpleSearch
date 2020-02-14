using System;
using Newtonsoft.Json;

namespace SimpleSearch.Messages
{
    public abstract class BaseMessage
    {
        [JsonProperty]
        public Guid Id { get; set; } = Guid.NewGuid();

        [JsonProperty]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow;
    }
}
