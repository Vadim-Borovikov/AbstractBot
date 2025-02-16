using System;
using AbstractBot.Bots;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;
using AbstractBot.Configs.MessageTemplates;
using JetBrains.Annotations;

namespace AbstractBot.Operations.Commands;

[PublicAPI]
public sealed class Help : CommandSimple
{
    protected override byte Order => Bot.ConfigBasic.HelpCommandMenuOrder;

    internal Help(BotBasic bot) : base(bot, "help", bot.ConfigBasic.TextsBasic.HelpCommandDescription) { }

    public void SetArgs(params object?[] args) => _messageArgs = args;

    protected override Task ExecuteAsync(Message message, User sender)
    {
        MessageTemplateText descriptions = GetOperationDescriptionsFor(sender.Id);
        MessageTemplateText text = descriptions;
        if (Bot.ConfigBasic.TextsBasic.HelpFormat is not null)
        {
            List<object?> args = new()
            {
                descriptions
            };
            args.AddRange(_messageArgs);
            text = Bot.ConfigBasic.TextsBasic.HelpFormat.Format(args.ToArray());
        }
        return text.SendAsync(Bot, message.Chat);
    }

    private MessageTemplateText GetOperationDescriptionsFor(long userId)
    {
        AccessData access = Bot.GetAccess(userId);

        List<MessageTemplateText> descriptions =
            Bot.Operations
               .Where(o => access.IsSufficientAgainst(o.AccessRequired))
               .Select(o => o.Description)
               .SkipNulls()
               .ToList();

        return MessageTemplateText.JoinTexts(descriptions.ToList());
    }

    private object?[] _messageArgs = Array.Empty<object?>();
}