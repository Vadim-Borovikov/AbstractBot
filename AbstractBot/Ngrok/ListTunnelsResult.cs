using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Ngrok;

internal sealed class ListTunnelsResult
{
    public sealed class Tunnel
    {
        [UsedImplicitly]
        public string? Proto;
        [UsedImplicitly]
        public string? PublicUrl;
    }

    [UsedImplicitly]
    public List<Tunnel?>? Tunnels;
}