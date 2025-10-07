using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateImage : MessageTemplate
{
    public string ImagePath { get; init; } = null!;

    public bool ShowCaptionAboveMedia;
    public bool HasSpoiler;

    public MessageTemplateImage() { }

    public MessageTemplateImage(string text, string imagePath, bool markdownV2 = false) : base(text, markdownV2)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImage(MessageTemplate prototype, string imagePath) : base(prototype)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImage(MessageTemplateImage prototype) : base(prototype)
    {
        ImagePath = prototype.ImagePath;
        HasSpoiler = prototype.HasSpoiler;
    }

    public override MessageTemplateImage Format(params object?[] args)
    {
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateImage(this)
        {
            MarkdownV2 = info.MarkdownV2,
            TextJoined = info.Text
        };
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendPhotoAsync(chat, ImagePath, KeyboardProvider, TextJoined, ParseMode,
            ReplyParameters, MessageThreadId, Entities, ShowCaptionAboveMedia, HasSpoiler, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, CancellationToken);
    }

    public Task<Message> EditMessageTextWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, TextJoined, ParseMode, Entities, null, keyboard,
            BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageMediaWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageMediaAsync(chat, messageId, ImagePath, TextJoined, ParseMode, keyboard,
            BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageCaptionWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageCaptionAsync(chat, messageId, TextJoined, ParseMode, Entities,
            ShowCaptionAboveMedia, keyboard, BusinessConnectionId, CancellationToken);
    }
}