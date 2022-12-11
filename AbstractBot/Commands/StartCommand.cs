using System;
using System.Threading.Tasks;
using AbstractBot.Operations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

public sealed class StartCommand : CommandOperation
{
    protected override byte MenuOrder => 0;

    internal StartCommand(BotBase bot) : base(bot, "start", "приветствие") { }

    protected override Task ExecuteAsync(Message message, long senderId, string? payload)
    {
        return payload is null
            ? Greet(message.Chat)
            : BotBase.OnStartCommand(this, message, senderId, payload);
    }

    internal async Task Greet(Chat chat)
    {
        await BotBase.UpdateCommandsFor(chat.Id);

        string text = $"{BotBase.About}";
        if (!string.IsNullOrWhiteSpace(BotBase.StartPostfix))
        {
            text += $"{Environment.NewLine}{BotBase.StartPostfix}";
        }
        await BotBase.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}