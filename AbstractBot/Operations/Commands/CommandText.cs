using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Configs;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class CommandText : CommandSimple
{
    protected CommandText(BotBasic bot, string command, string description, MessageTemplate messageTemplate)
        : base(bot, command, description)
    {
        _bot = bot;
        _messageTemplate = messageTemplate;
    }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        return _messageTemplate.SendAsync(_bot, message.Chat);
    }

    private readonly BotBasic _bot;
    private readonly MessageTemplate _messageTemplate;
}