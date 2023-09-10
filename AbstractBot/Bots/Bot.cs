using AbstractBot.Configs;
using AbstractBot.Operations.Commands;
using GryphonUtilities;
using JetBrains.Annotations;
using System.Threading.Tasks;
using AbstractBot.Operations.Infos;
using Telegram.Bot.Types;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class Bot<TConfig, TTexts, TData, TStartInfo> : BotBasic
    where TConfig : Config<TTexts>
    where TTexts : Texts
    where TData : SaveData, new()
    where TStartInfo : class, ICommandInfo<TStartInfo>
{
    public new readonly TConfig Config;

    protected internal readonly SaveManager<TData> SaveManager;

    protected Bot(TConfig config) : base(config)
    {
        Config = config;

        SaveManager = new SaveManager<TData>(Config.SavePath, TimeManager);
        SaveManager.Load();

        Start = new Start<TStartInfo>(this, OnStartCommand);
        Operations.Add(Start);
    }

    protected internal virtual Task OnStartCommand(TStartInfo info, Message message, User sender)
    {
        return Start.Greet(message.Chat);
    }

    protected readonly Start<TStartInfo> Start;
}