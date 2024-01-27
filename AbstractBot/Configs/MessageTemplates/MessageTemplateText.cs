using System.Collections.Generic;
using System.Linq;
using AbstractBot.Bots;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace AbstractBot.Configs.MessageTemplates;

[PublicAPI]
public class MessageTemplateText : MessageTemplate
{
    public bool? DisableWebPagePreview;

    public MessageTemplateText() { }

    public MessageTemplateText(string text, bool markdownV2 = false) : base(text, markdownV2) { }

    public MessageTemplateText(MessageTemplateText prototype) : base(prototype)
    {
        DisableWebPagePreview = prototype.DisableWebPagePreview;
    }

    public static MessageTemplateText JoinTexts(IList<MessageTemplateText> elements)
    {
        bool shouldEscape = elements.Any(e => e.MarkdownV2);
        return new MessageTemplateText
        {
            Text = elements.Select(e => shouldEscape ? e.EscapeIfNeeded() : e.TextJoined),
            MarkdownV2 = shouldEscape
        };
    }

    public override MessageTemplateText Format(params object?[] args)
    {
        return new MessageTemplateText(this)
        {
            TextJoined = FormatText(args)
        };
    }

    protected override Task<Message> SendAsync(BotBasic bot, Chat chat, string text)
    {
        return bot.SendTextMessageAsync(chat, text, KeyboardProvider, ParseMode.MarkdownV2, MessageThreadId, Entities,
            DisableWebPagePreview, DisableNotification, ProtectContent, ReplyToMessageId, AllowSendingWithoutReply,
            CancellationToken);
    }
}