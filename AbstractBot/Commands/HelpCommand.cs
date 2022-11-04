using GryphonUtilities;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class HelpCommand<TBot, TConfig> : CommandBase<TBot, TConfig>
    where TBot : BotBase<TBot, TConfig>
    where TConfig : Config
{
    public HelpCommand(TBot bot) : base(bot, "help", "инструкция") { }

    public override async Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        User user = message.From.GetValue(nameof(message.From));
        string text = $"{Bot.About}{Environment.NewLine}{Environment.NewLine}";
        if (!string.IsNullOrWhiteSpace(Bot.HelpPrefix))
        {
            text += $"{Bot.HelpPrefix}{Environment.NewLine}{Environment.NewLine}";
        }

        text += Bot.GetCommandsDescriptionFor(user.Id);
        await Bot.SendTextMessageAsync(message.Chat, text, ParseMode.MarkdownV2);
    }
}