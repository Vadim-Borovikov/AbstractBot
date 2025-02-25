using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Bots;

[PublicAPI]
public sealed class Greeter: IGreeter
{
    public Greeter(IUpdateSender updateSender, MessageTemplate greeting)
    {
        _updateSender = updateSender;
        _greeting = greeting;
    }

    public Task Greet(Message message, User from) => _greeting.SendAsync(_updateSender, message.Chat);

    private readonly IUpdateSender _updateSender;
    private readonly MessageTemplate _greeting;
}