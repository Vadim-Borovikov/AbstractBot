using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using AbstractBot.Legacy.Bots;
using AbstractBot.Legacy.Configs;
using AbstractBot.Legacy.Configs.MessageTemplates;

namespace AbstractBot.Legacy.Operations.Commands;

[PublicAPI]
public sealed class Help : CommandSimple
{
    protected override byte Order => _config.HelpCommandMenuOrder;

    internal Help(ConfigBasic config)
        : base(config.TextsBasic.CommandDescriptionFormat, "help", config.TextsBasic.HelpCommandDescription)
    {
        _config = config;
    }

    public void SetArgs(params object?[] args) => _messageArgs = args;

    protected override Task ExecuteAsync(BotBasic bot, Message message, User sender)
    {
        MessageTemplateText descriptions = GetOperationDescriptionsFor(bot, sender.Id);
        MessageTemplateText text = descriptions;
        if (_config.TextsBasic.HelpFormat is not null)
        {
            List<object?> args = new()
            {
                descriptions
            };
            args.AddRange(_messageArgs);
            text = _config.TextsBasic.HelpFormat.Format(args.ToArray());
        }
        return text.SendAsync(bot, message.Chat);
    }

    private MessageTemplateText GetOperationDescriptionsFor(BotBasic bot, long userId)
    {
        AccessData access = bot.GetAccess(userId);

        List<MessageTemplateText> descriptions =
            bot.Operations
               .Where(o => access.IsSufficientAgainst(o.AccessRequired))
               .Select(o => o.Description)
               .SkipNulls()
               .ToList();

        return MessageTemplateText.JoinTexts(descriptions.ToList());
    }

    private object?[] _messageArgs = Array.Empty<object?>();
    private readonly ConfigBasic _config;
}