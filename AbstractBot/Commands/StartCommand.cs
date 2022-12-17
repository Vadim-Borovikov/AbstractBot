using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Operations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

public sealed class StartCommand : CommandOperation
{
    protected override byte MenuOrder => 0;

    internal StartCommand(Bot bot) : base(bot, "start", "приветствие") { }

    protected override Task ExecuteAsync(Message message, long senderId, string? payload)
    {
        return payload is null
            ? Greet(message.Chat)
            : Bot.OnStartCommand(this, message, senderId, payload);
    }

    internal async Task Greet(Chat chat)
    {
        await Bot.UpdateCommandsFor(chat.Id);

        string text = $"{Bot.About}";
        if (!string.IsNullOrWhiteSpace(Bot.StartPostfix))
        {
            text += $"{Environment.NewLine}{Bot.StartPostfix}";
        }
        await Bot.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}