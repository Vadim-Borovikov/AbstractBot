using GryphonUtilities;
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
        await Bot.SendTextMessageAsync(message.Chat, Bot.GetDescriptionFor(user.Id), ParseMode.MarkdownV2);
    }
}