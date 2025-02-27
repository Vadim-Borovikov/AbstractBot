using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.Operations.Commands;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot;

[PublicAPI]
public abstract class Bot
{
    public readonly IBotCore Core;
    public readonly ICommands Commands;

    protected Bot(IBotCore core, ICommands commands, IStartCommand start, Help help)
    {
        Core = core;

        Commands = commands;

        Core.UpdateReceiver.Operations.Add(start);
        Core.UpdateReceiver.Operations.Add(help);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await Core.Connection.StartAsync(cancellationToken);

        await Core.Logging.StartAsync(cancellationToken);

        await Commands.UpdateForAll(cancellationToken);
    }

    public virtual Task StopAsync(CancellationToken cancellationToken) => Core.Connection.StopAsync(cancellationToken);

    public virtual void Update(Update update) => Core.UpdateReceiver.Update(update);
}