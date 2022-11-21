using System.Collections.Generic;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace AbstractBot.Ngrok;

[PublicAPI]
internal sealed class ListTunnelsResult
{
    [PublicAPI]
    public sealed class Tunnel
    {
        public string? Proto;

        [JsonPropertyName("public_url")]
        public string? PublicUrl;
    }

    public List<Tunnel?>? Tunnels;
}