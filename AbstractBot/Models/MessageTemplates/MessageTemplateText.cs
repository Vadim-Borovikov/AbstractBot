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

    public MessageTemplateText(string text, bool escaped = false) : base(text, escaped) { }

    public MessageTemplateText(MessageTemplate prototype) : base(prototype) { }

    public MessageTemplateText(MessageTemplateText prototype) : base(prototype)
    {
        LinkPreviewOptions = prototype.LinkPreviewOptions;
    }

    public static MessageTemplateText JoinTexts(IList<MessageTemplateText> elements)
    {
        bool shouldEscape = elements.Any(e => e.Escaped);
        IEnumerable<string> lines = elements.Select(e => shouldEscape ? e.EscapeIfNeeded() : e.TextJoined);
        return new MessageTemplateText
        {
            TextJoined = GryphonUtilities.Helpers.Text.JoinLines(lines),
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
            Entities, BusinessConnectionId, CancellationToken);
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