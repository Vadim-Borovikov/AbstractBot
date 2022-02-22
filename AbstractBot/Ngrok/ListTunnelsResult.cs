using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace AbstractBot.Ngrok;

[UsedImplicitly]
internal sealed class ListTunnelsResult
{
    public sealed class Tunnel
    {
        [JsonProperty]
        public string? Proto { get; set; }

        [JsonProperty]
        public string? PublicUrl { get; set; }
    }

    [JsonProperty]
    public List<Tunnel?>? Tunnels { get; set; }
}