using GryphonUtilities;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class HelpCommand : CommandBase
{
    public HelpCommand(BotBase bot) : base(bot, "help", "инструкция") { }

    public override async Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        User user = message.From.GetValue(nameof(message.From));
        string text = $"{BotBase.About}{Environment.NewLine}{Environment.NewLine}";
        if (!string.IsNullOrWhiteSpace(BotBase.HelpPrefix))
        {
            text += $"{BotBase.HelpPrefix}{Environment.NewLine}{Environment.NewLine}";
        }

        text += BotBase.GetCommandsDescriptionFor(user.Id);
        await BotBase.SendTextMessageAsync(message.Chat, text, ParseMode.MarkdownV2);
    }
}