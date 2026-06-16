using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateText : MessageTemplateMarkdownV2
{
    public LinkPreviewOptions? LinkPreviewOptions;

    public MessageTemplateText() { }

    public MessageTemplateText(string text, bool escaped = false) : base(text, escaped) { }

    public MessageTemplateText(MessageTemplate prototype) : base(prototype) { }

    public MessageTemplateText(MessageTemplateText prototype) : base(prototype)
    {
        LinkPreviewOptions = prototype.LinkPreviewOptions;
    }

    public static MessageTemplateText JoinTexts(IReadOnlyCollection<MessageTemplateText> elements,
        string separator = "\n")
    {
        bool shouldEscape = elements.Any(e => e.Escaped);
        IEnumerable<string> parts = elements.Select(e => shouldEscape ? e.EscapeIfNeeded() : e.TextJoined);
        return new MessageTemplateText
        {
            TextJoined = string.Join(separator, parts),
            Escaped = shouldEscape
        };
    }

    public override MessageTemplateText Format(params object?[] args)
    {
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateText(this)
        {
            Escaped = info.Escaped,
            TextJoined = info.Text
        };
    }

    public Task<Message> EditMessageWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, TextJoined, ParseMode, keyboard, LinkPreviewOptions,
            Entities, null, BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageCaptionWithSelfAsync(IUpdateSender updateSender, Chat chat,
        bool showCaptionAboveMedia, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageCaptionAsync(chat, messageId, TextJoined, ParseMode, keyboard, Entities,
            showCaptionAboveMedia, BusinessConnectionId, CancellationToken);
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendTextMessageAsync(chat, TextJoined, KeyboardProvider, ParseMode, ReplyParameters,
            LinkPreviewOptions, MessageThreadId, Entities, DisableNotification, ProtectContent, MessageEffectId,
            BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId, SuggestedPostParameters,
            CancellationToken);
    }
}