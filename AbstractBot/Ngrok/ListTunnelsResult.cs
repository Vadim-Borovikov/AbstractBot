using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Ngrok;

[PublicAPI]
internal sealed class ListTunnelsResult
{
    [PublicAPI]
    public sealed class Tunnel
    {
        public string? Proto;
        public string? PublicUrl;
    }

    public List<Tunnel?>? Tunnels;
}