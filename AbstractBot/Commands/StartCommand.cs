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
        await Bot.SendTextMessageAsync(message.Chat, Bot.About, ParseMode.MarkdownV2);
    }
}