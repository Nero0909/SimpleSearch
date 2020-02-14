using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SimpleSearch.Messages.Abstractions;

namespace SimpleSearch.Messages
{
    public class FragmentTokenizedEvent : FileMessage
    {
        [JsonProperty]
        public long Offset { get; set; }

        [JsonProperty]
        public long Length { get; set; }

        [JsonProperty]
        public IEnumerable<Token> Tokens { get; set; } = Array.Empty<Token>();
    }

    public class Token
    {
        [JsonProperty]
        public string Tag { get; set; }

        [JsonProperty]
        public int Frequency { get; set; }
    }
}