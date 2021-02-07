using System.Collections.Generic;
using Newtonsoft.Json;

namespace AbstractBot
{
    public class Config
    {
        [JsonProperty]
        public string Token { get; set; }

        [JsonProperty]
        public string Host { get; set; }

        [JsonProperty]
        public int Port { get; set; }

        internal string Url => $"{Host}:{Port}/{Token}";

        [JsonProperty]
        public List<string> DescriptionStart { get; set; }

        [JsonProperty]
        public List<string> DescriptionEnd { get; set; }

        [JsonProperty]
        public string SystemTimeZoneId { get; set; }

        [JsonProperty]
        public List<int> AdminIds { get; set; }

        [JsonProperty]
        public string DontUnderstandStickerFileId { get; set; }

        [JsonProperty]
        public string ForbiddenStickerFileId { get; set; }
    }
}