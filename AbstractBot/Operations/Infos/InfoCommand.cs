using JetBrains.Annotations;

namespace AbstractBot.Operations.Infos;

[PublicAPI]
public class InfoCommand
{
    public readonly string Payload;
    public InfoCommand(string payload) => Payload = payload;
}