using System.Collections.Generic;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplateMarkdownV2 : MessageTemplate
{
    public IEnumerable<MessageEntity>? Entities;
    protected ParseMode ParseMode => Escaped ? ParseMode.MarkdownV2 : ParseMode.None;

    protected MessageTemplateMarkdownV2() { }

    protected MessageTemplateMarkdownV2(string text, bool escaped = false) : base(text, escaped) { }

    protected MessageTemplateMarkdownV2(MessageTemplate prototype) : base(prototype) { }

    protected MessageTemplateMarkdownV2(MessageTemplateMarkdownV2 prototype) : base(prototype)
    {
        Entities = prototype.Entities;
    }
}