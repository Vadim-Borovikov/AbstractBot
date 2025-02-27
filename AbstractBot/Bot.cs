using System.Threading;
using System.Threading.Tasks;
using AbstractBot.Interfaces;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.Operations.Commands;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot;

[PublicAPI]
public abstract class Bot
{
    public readonly IBotCore Core;

    protected Bot(IBotCore core, IStartCommand start)
    {
        Core = core;

        Core.UpdateReceiver.Operations.Add(start);

        Help help = new(Core.Accesses, Core.UpdateSender, Core.UpdateReceiver,
            Core.Config.Texts.HelpCommandDescription, Core.SelfUsername, Core.Config.Texts.HelpFormat);

        Core.UpdateReceiver.Operations.Add(help);
    }

    public virtual async Task StartAsync(CancellationToken cancellationToken)
    {
        await Core.Connection.StartAsync(cancellationToken);

        await Core.Logging.StartAsync(cancellationToken);

        await Core.Commands.UpdateCommands(cancellationToken);
    }

    public virtual Task StopAsync(CancellationToken cancellationToken) => Core.Connection.StopAsync(cancellationToken);

    public virtual void Update(Update update) => Core.UpdateReceiver.Update(update);
}