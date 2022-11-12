using JetBrains.Annotations;

namespace AbstractBot;

[PublicAPI]
public abstract class BotBaseCustom<TConfig> : BotBase where TConfig : Config
{
    public readonly TConfig Config;

    protected BotBaseCustom(TConfig config) : base(config) => Config = config;
}