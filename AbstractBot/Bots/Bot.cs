using AbstractBot.Configs;
using AbstractBot.Save;
using GryphonUtilities;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class Bot<TConfig, TTexts, TData> : BotBase
    where TConfig : CustomConfig<TTexts>
    where TTexts : Texts
    where TData : Data, new()
{
    public new readonly TConfig Config;

    protected SaveManager<TData> SaveManager { get; init; }

    protected Bot(TConfig config) : base(config)
    {
        Config = config;

        SaveManager = new SaveManager<TData>(Config.SavePath, TimeManager);
        SaveManager.Load();
    }
}