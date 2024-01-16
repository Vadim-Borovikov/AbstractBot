using AbstractBot.Bots;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GryphonUtilities.Extensions;
using Telegram.Bot.Types;
using AbstractBot.Configs.MessageTemplates;

namespace AbstractBot.Operations.Commands;

internal sealed class Help : CommandSimple
{
    protected override byte Order => Bot.Config.HelpCommandMenuOrder;

    public Help(BotBasic bot) : base(bot, "help", bot.Config.Texts.HelpCommandDescription) { }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        MessageTemplateText descriptions = GetOperationDescriptionsFor(sender.Id);
        if (Bot.Config.Texts.HelpFormat is not null)
        {
            descriptions = Bot.Config.Texts.HelpFormat.Format(descriptions);
        }
        return descriptions.SendAsync(Bot, message.Chat);
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
}