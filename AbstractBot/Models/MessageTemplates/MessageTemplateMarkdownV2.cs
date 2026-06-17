using JetBrains.Annotations;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplateMarkdownV2 : MessageTemplate
{
    public IEnumerable<MessageEntity>? Entities;

    protected MessageTemplateMarkdownV2(string text) : base(text) { }
    protected MessageTemplateMarkdownV2(MessageTemplate prototype) : base(prototype) { }
    protected MessageTemplateMarkdownV2(MessageTemplateMarkdownV2 prototype) : base(prototype)
    {
        Entities = prototype.Entities;
    }
}