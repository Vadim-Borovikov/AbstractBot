using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateText : MessageTemplateMarkdownV2
{
    public LinkPreviewOptions? LinkPreviewOptions;

    public MessageTemplateText(string text) : base(text) { }
    public MessageTemplateText(MessageTemplate prototype) : base(prototype) { }
    public MessageTemplateText(MessageTemplateText prototype) : base(prototype)
    {
        LinkPreviewOptions = prototype.LinkPreviewOptions;
    }

    public Task<Message> EditMessageWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, Text, ParseMode.MarkdownV2, keyboard,
            LinkPreviewOptions, Entities, null, BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageCaptionWithSelfAsync(IUpdateSender updateSender, Chat chat,
        bool showCaptionAboveMedia, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageCaptionAsync(chat, messageId, Text, ParseMode.MarkdownV2, keyboard, Entities,
            showCaptionAboveMedia, BusinessConnectionId, CancellationToken);
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendTextMessageAsync(chat, Text, KeyboardProvider, ParseMode.MarkdownV2, ReplyParameters,
            LinkPreviewOptions, MessageThreadId, Entities, DisableNotification, ProtectContent, MessageEffectId,
            BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId, SuggestedPostParameters,
            CancellationToken);
    }
}