using JetBrains.Annotations;
using System.Threading.Tasks;
using GryphonUtilities;
using Telegram.Bot.Types;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Operations.Commands;
using Telegram.Bot;
using AbstractBot.Interfaces.Operations.Commands;
using AbstractBot.Models.Operations.Commands.Start;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public abstract class Bot<TConfig, TTexts, TContext, TContextData, TMetaContext, TSaveData, TStartData> : BotBasic
    where TConfig : Config<TTexts>
    where TTexts : TextsBasic
    where TContext : class, IContext<TContext, TContextData, TMetaContext>
    where TContextData : class
    where TMetaContext : class
    where TSaveData : SaveData<TContextData>, new()
    where TStartData : class, ICommandData<TStartData>
{
    public readonly TConfig Config;

    public Dictionary<long, TContext> Contexts { get; init; } = new();
    public SaveManager<TSaveData> SaveManager { get; init; }

    protected Bot(TConfig config, TelegramBotClient client, User self, string host) : base(config, client, self, host)
    {
        Config = config;

        SaveManager = new SaveManager<TSaveData>(Config.SavePath, Clock, AfterLoad, BeforeSave);

        Start = new Start<TStartData>(Config.Texts, OnStartCommand);

        Operations.Add(Start);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        await base.StartAsync(cancellationToken);
        SaveManager.Load();
    }

    protected internal virtual Task OnStartCommand(TStartData data, Message message, User sender)
    {
        return Start.Greet(this, message.Chat, sender);
    }

    protected virtual void AfterLoad()
    {
        Contexts.Clear();

        TMetaContext? meta = GetMetaContext();

        foreach (long id in SaveManager.SaveData.ContextDatas.Keys)
        {
            TContextData contextData = SaveManager.SaveData.ContextDatas[id];
            TContext? context = TContext.Load(contextData, meta);
            if (context is not null)
            {
                Contexts[id] = context;
            }
        }
    }

    protected virtual void BeforeSave()
    {
        List<long> toRemove = SaveManager.SaveData.ContextDatas.Keys.Where(k => !Contexts.ContainsKey(k)).ToList();

        foreach (long id in toRemove)
        {
            SaveManager.SaveData.ContextDatas.Remove(id);
        }

        foreach (long id in Contexts.Keys)
        {
            TContextData? data = Contexts[id].Save();
            if (data is not null)
            {
                SaveManager.SaveData.ContextDatas[id] = data;
            }
        }
    }

    protected virtual TMetaContext? GetMetaContext() => null;

    protected readonly Start<TStartData> Start;
}