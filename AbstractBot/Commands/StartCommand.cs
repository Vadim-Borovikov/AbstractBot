using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class StartCommand<TBot, TConfig> : CommandBase<TBot, TConfig>
    where TBot : BotBase<TBot, TConfig>
    where TConfig : Config
{
    public StartCommand(TBot bot) : base(bot, "start", "приветствие") { }

    public override async Task ExecuteAsync(Message message, bool fromChat, string? payload)
    {
        await Bot.SetCommandsForAsync(message.Chat);
        string text = $"{Bot.About}";
        if (!string.IsNullOrWhiteSpace(Bot.Config.StartPostfix))
        {
            text += $"{Environment.NewLine}{Bot.Config.StartPostfix}";
        }
        await Bot.SendTextMessageAsync(message.Chat, text, ParseMode.MarkdownV2);
    }
}