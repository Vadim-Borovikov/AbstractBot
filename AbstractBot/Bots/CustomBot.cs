using AbstractBot.Configs;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class CustomBot<T> : Bot where T : Config
{
    public new readonly T Config;

    protected CustomBot(T config) : base(config) => Config = config;
}