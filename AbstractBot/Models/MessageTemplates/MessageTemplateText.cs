using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateText : MessageTemplate
{
    public LinkPreviewOptions? LinkPreviewOptions;

    public MessageTemplateText() { }

    public MessageTemplateText(string text, bool markdownV2 = false) : base(text, markdownV2) { }

    public MessageTemplateText(MessageTemplate prototype) : base(prototype) { }

    public MessageTemplateText(MessageTemplateText prototype) : base(prototype)
    {
        LinkPreviewOptions = prototype.LinkPreviewOptions;
    }

    public static MessageTemplateText JoinTexts(IList<MessageTemplateText> elements)
    {
        bool shouldEscape = elements.Any(e => e.MarkdownV2);
        IEnumerable<string> lines = elements.Select(e => shouldEscape ? e.EscapeIfNeeded() : e.TextJoined);
        return new MessageTemplateText
        {
            TextJoined = GryphonUtilities.Helpers.Text.JoinLines(lines),
            MarkdownV2 = shouldEscape
        };
    }

    public override MessageTemplateText Format(params object?[] args)
    {
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateText(this)
        {
            MarkdownV2 = info.MarkdownV2,
            TextJoined = info.Text
        };
    }

    public Task<Message> EditMessageWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, TextJoined, ParseMode, Entities, LinkPreviewOptions,
            keyboard, BusinessConnectionId, CancellationToken);
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendTextMessageAsync(chat, TextJoined, KeyboardProvider, ParseMode, ReplyParameters,
            LinkPreviewOptions, MessageThreadId, Entities, DisableNotification, ProtectContent, MessageEffectId,
            BusinessConnectionId, AllowPaidBroadcast, CancellationToken);
    }
}