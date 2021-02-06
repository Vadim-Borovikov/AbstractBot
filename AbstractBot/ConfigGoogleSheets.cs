using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "ClassCanBeSealed.Global")]
    public class ConfigGoogleSheets : Config
    {
        [JsonProperty]
        public Dictionary<string, string> GoogleCredential { get; set; }

        [JsonProperty]
        public string ApplicationName { get; set; }

        [JsonProperty]
        public string GoogleSheetId { get; set; }
    }
}