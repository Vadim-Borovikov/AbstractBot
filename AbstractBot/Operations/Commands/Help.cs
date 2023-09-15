using AbstractBot.Bots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GryphonUtilities;
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

        string text = Bot.Config.Texts.HelpLinesFormatMarkdownV2 is null
            ? $"{Bot.About}{Environment.NewLine}{descriptions}"
            : Text.FormatLines(Bot.Config.Texts.HelpLinesFormatMarkdownV2, Bot.About, descriptions);

        return Bot.SendTextMessageAsync(message.Chat, text, ParseMode.MarkdownV2);
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