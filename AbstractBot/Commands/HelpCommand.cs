using AbstractBot.Bots;
using AbstractBot.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GryphonUtilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Commands;

internal sealed class HelpCommand : CommandOperation
{
    protected override byte MenuOrder => Bot.Config.HelpCommandMenuOrder;

    public HelpCommand(BotBase bot) : base(bot, "help", bot.Config.Texts.HelpCommandDescription) { }

    protected override Task ExecuteAsync(Message message, long _, string? __)
    {
        string text = $"{Bot.About}{Environment.NewLine}";
        if (Bot.Config.Texts.HelpPrefixLinesMarkdownV2 is not null)
        {
            text += $"{Text.JoinLines(Bot.Config.Texts.HelpPrefixLinesMarkdownV2)}{Environment.NewLine}";
        }

        text += GetOperationsDescriptionFor(message.Chat.Id);

        if (Bot.Config.Texts.HelpPostfixLinesMarkdownV2 is not null)
        {
            text += $"{Text.JoinLines(Bot.Config.Texts.HelpPostfixLinesMarkdownV2)}";
        }

        return Bot.SendTextMessageAsync(message.Chat, text, ParseMode.MarkdownV2);
    }

    private string GetOperationsDescriptionFor(long userId)
    {
        Access access = Bot.GetMaximumAccessFor(userId);

        StringBuilder builder = new();
        List<Operation> operations = Bot.Operations.Where(o => o.MenuDescription is not null).ToList();
        List<Operation> userOperations = operations.Where(o => o.AccessLevel == Access.User).ToList();
        if (access != Access.User)
        {
            List<Operation> adminOperations = operations.Where(o => o.AccessLevel == Access.Admin).ToList();
            if (access == Access.SuperAdmin)
            {
                List<Operation> superAdminOperations =
                    operations.Where(o => o.AccessLevel == Access.SuperAdmin).ToList();
                if (superAdminOperations.Any())
                {
                    builder.AppendLine(
                        superAdminOperations.Count > 1 ? "Команды суперадмина:" : "Команда суперадмина:");
                    foreach (Operation operation in superAdminOperations)
                    {
                        builder.AppendLine(operation.MenuDescription);
                    }
                    if (adminOperations.Any() || userOperations.Any())
                    {
                        builder.AppendLine();
                    }
                }
            }

            if (adminOperations.Any())
            {
                builder.AppendLine(adminOperations.Count > 1 ? "Админские команды:" : "Админская команда:");
                foreach (Operation operation in adminOperations)
                {
                    builder.AppendLine(operation.MenuDescription);
                }
                if (userOperations.Any())
                {
                    builder.AppendLine();
                }
            }
        }

        if (userOperations.Any())
        {
            builder.AppendLine(userOperations.Count > 1 ? "Команды:" : "Команда:");
            foreach (Operation operation in userOperations)
            {
                builder.AppendLine(operation.MenuDescription);
            }
        }

        return builder.ToString();
    }
}