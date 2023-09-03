using AbstractBot.Configs;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class CustomBot<TConfig, TTexts> : Bot
    where TConfig : CustomConfig<TTexts>
    where TTexts : Texts
{
    public new readonly TConfig Config;

    protected CustomBot(TConfig config) : base(config) => Config = config;
}