using AbstractBot.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbstractBot.Configs;
using AbstractBot.Extensions;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations.Commands;

internal sealed class Help : CommandSimple
{
    protected override byte Order => Bot.Config.HelpCommandMenuOrder;

    public Help(BotBasic bot) : base(bot, "help", bot.Config.Texts.HelpCommandDescription) { }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        string descriptions = GetOperationsDescriptionFor(sender.Id);

        if (Bot.Config.Texts.HelpFormat is null)
        {
            return Bot.SendTextMessageAsync(message.Chat, descriptions.Escape(), parseMode: ParseMode.MarkdownV2);
        }

        MessageText formatted = Bot.Config.Texts.HelpFormat.Format(descriptions);
        return formatted.SendAsync(Bot, message.Chat);
    }

    private string GetOperationsDescriptionFor(long userId)
    {
        int access = Bot.GetAccess(userId);

        IEnumerable<string> descriptions =
            Bot.Operations
               .Where(o => AccessHelpers.IsSufficient(access, o.AccessRequired))
               .Select(o => o.MenuDescription)
               .RemoveNulls();

        return string.Join(Environment.NewLine, descriptions);
    }
}