using System.Threading.Tasks;
using AbstractBot.Legacy.Bots;
using AbstractBot.Legacy.Configs.MessageTemplates;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Legacy.Operations.Commands;

[PublicAPI]
public abstract class CommandText : CommandSimple
{
    protected CommandText(MessageTemplateText descriptionFormat, string command, string description,
        MessageTemplate messageTemplate)
        : base(descriptionFormat, command, description)
    {
        _messageTemplate = messageTemplate;
    }

    protected override Task ExecuteAsync(BotBasic bot, Message message, User sender)
    {
        return _messageTemplate.SendAsync(bot, message.Chat);
    }

    private readonly MessageTemplate _messageTemplate;
}