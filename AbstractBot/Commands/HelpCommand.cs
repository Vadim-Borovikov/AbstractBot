using AbstractBot.Operations;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class HelpCommand : CommandOperation
{
    protected internal override byte MenuOrder => 1;

    public HelpCommand(BotBase bot) : base(bot, "help", "инструкция") { }

    protected override Task ExecuteAsync(Message message, Chat sender, string? _)
    {
        Chat chat = BotBase.GetReplyChatFor(message, sender);

        string text = $"{BotBase.About}{Environment.NewLine}{Environment.NewLine}";
        if (!string.IsNullOrWhiteSpace(BotBase.HelpPrefix))
        {
            text += $"{BotBase.HelpPrefix}{Environment.NewLine}{Environment.NewLine}";
        }

        text += BotBase.GetOperationsDescriptionFor(chat.Id);
        return BotBase.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}