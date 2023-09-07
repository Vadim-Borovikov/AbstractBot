using AbstractBot.Configs;
using GryphonUtilities;
using JetBrains.Annotations;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class Bot<TConfig, TTexts, TData> : BotBasic
    where TConfig : Config<TTexts>
    where TTexts : Texts
    where TData : SaveData, new()
{
    public new readonly TConfig Config;

    protected internal readonly SaveManager<TData> SaveManager;

    protected Bot(TConfig config) : base(config)
    {
        Config = config;

        SaveManager = new SaveManager<TData>(Config.SavePath, TimeManager);
        SaveManager.Load();
    }
}