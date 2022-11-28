using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class HelpCommand : CommandBase
{
    public HelpCommand(BotBase bot) : base(bot, "help", "инструкция") { }

    public override async Task ExecuteAsync(Message message, Chat chat, string? payload)
    {
        string text = $"{BotBase.About}{Environment.NewLine}{Environment.NewLine}";
        if (!string.IsNullOrWhiteSpace(BotBase.HelpPrefix))
        {
            text += $"{BotBase.HelpPrefix}{Environment.NewLine}{Environment.NewLine}";
        }

        text += BotBase.GetCommandsDescriptionFor(chat.Id);
        await BotBase.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}