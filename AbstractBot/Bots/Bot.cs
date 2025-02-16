using AbstractBot.Configs;
using AbstractBot.Operations.Commands;
using JetBrains.Annotations;
using System.Threading.Tasks;
using GryphonUtilities;
using Telegram.Bot.Types;
using AbstractBot.Operations.Data;

namespace AbstractBot.Bots;

[PublicAPI]
public abstract class Bot<TConfig, TTexts, TSaveData, TStartData> : BotBasic
    where TConfig : Config<TTexts>
    where TTexts : TextsBasic
    where TSaveData : new()
    where TStartData : class, ICommandData<TStartData>
{
    public readonly TConfig Config;

    protected internal readonly SaveManager<TSaveData> SaveManager;

    protected Bot(TConfig config) : base(config)
    {
        Config = config;

        SaveManager = new SaveManager<TSaveData>(Config.SavePath, Clock, AfterLoad, BeforeSave);
        SaveManager.Load();

        Start = new Start<TStartData>(this, OnStartCommand);
        Operations.Add(Start);
    }

    protected internal virtual Task OnStartCommand(TStartData data, Message message, User sender)
    {
        return Start.Greet(message.Chat, sender);
    }

    protected readonly Start<TStartData> Start;
}