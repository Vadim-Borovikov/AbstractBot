using JetBrains.Annotations;

namespace AbstractBot.Save;

[PublicAPI]
public interface IConfigSave
{
    public string SavePath { get; init; }
}