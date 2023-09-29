using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Configs;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public abstract class CommandText : CommandSimple
{
    protected CommandText(BotBasic bot, string command, string description, string text)
        : base(bot, command, description)
    {
        _bot = bot;
        _text = text;
    }

    protected CommandText(BotBasic bot, string command, string description, MessageText messageText)
        : base(bot, command, description)
    {
        _bot = bot;
        _messageText = messageText;
    }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        return _messageText is null
            ? _bot.SendTextMessageAsync(message.Chat, _text.Denull(), parseMode: ParseMode.MarkdownV2)
            : _messageText.SendAsync(_bot, message.Chat);
    }

    private readonly BotBasic _bot;
    private readonly MessageText? _messageText;
    private readonly string? _text;
}