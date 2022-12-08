using System;
using System.Threading.Tasks;
using AbstractBot.Operations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class StartCommand : CommandOperation
{
    protected internal override byte MenuOrder => 0;

    public StartCommand(BotBase bot) : base(bot, "start", "приветствие") { }

    protected override async Task ExecuteAsync(Message message, Chat sender, string? _)
    {
        Chat chat = BotBase.GetReplyChatFor(message, sender);

        await BotBase.UpdateCommandsFor(chat);

        string text = $"{BotBase.About}";
        if (!string.IsNullOrWhiteSpace(BotBase.StartPostfix))
        {
            text += $"{Environment.NewLine}{BotBase.StartPostfix}";
        }
        await BotBase.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}