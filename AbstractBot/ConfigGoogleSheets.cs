using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot;

[PublicAPI]
public class ConfigGoogleSheets : Config
{
    [JsonProperty]
    public Dictionary<string, string>? GoogleCredential { get; set; }

    [JsonProperty]
    public string? ApplicationName { get; set; }

    [JsonProperty]
    public string? GoogleSheetId { get; set; }
}