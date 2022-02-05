using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace AbstractBot.Ngrok;

[UsedImplicitly]
internal sealed class ListTunnelsResult
{
    [UsedImplicitly]
    public sealed class Tunnel
    {
        public string? Proto { get; [UsedImplicitly] set; }

        [JsonPropertyName("public_url")]
        public string? PublicUrl { get; [UsedImplicitly] set; }
    }

    [UsedImplicitly]
    public List<Tunnel>? Tunnels { get; set; }
}