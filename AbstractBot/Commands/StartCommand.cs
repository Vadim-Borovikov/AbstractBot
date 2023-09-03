using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Operations;
using GryphonUtilities;
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
        if (Bot.Config.Texts.StartPostfixLines is not null)
        {
            text += $"{Environment.NewLine}{Text.JoinLines(Bot.Config.Texts.StartPostfixLines)}";
        }
        await Bot.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}