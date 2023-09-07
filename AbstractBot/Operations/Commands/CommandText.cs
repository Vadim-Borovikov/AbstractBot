using System.Threading.Tasks;
using AbstractBot.Bots;
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

    protected override Task ExecuteAsync(Message message, User sender)
    {
        return _bot.SendTextMessageAsync(message.Chat, _text, ParseMode.MarkdownV2);
    }

    private readonly BotBasic _bot;
    private readonly string _text;
}