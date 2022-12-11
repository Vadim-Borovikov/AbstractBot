using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public abstract class BotBaseCustom<T> : BotBase where T : Config
{
    public readonly T Config;

    protected BotBaseCustom(T config) : base(config) => Config = config;
}