using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateImagePath : MessageTemplateImage
{
    public string ImagePath { get; init; }

    public MessageTemplateImagePath(string text, string imagePath) : base(text) => ImagePath = imagePath;
    public MessageTemplateImagePath(MessageTemplate prototype, string imagePath) : base(prototype)
    {
        ImagePath = imagePath;
    }
    public MessageTemplateImagePath(MessageTemplateImagePath prototype) : base(prototype)
    {
        ImagePath = prototype.ImagePath;
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendPhotoAsync(chat, ImagePath, KeyboardProvider, Text, ParseMode.MarkdownV2,
            ReplyParameters, MessageThreadId, Entities, ShowCaptionAboveMedia, HasSpoiler, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId,
            SuggestedPostParameters, CancellationToken);
    }

    public Task<Message> EditMessageMediaWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageMediaAsync(chat, messageId, ImagePath, Text, ParseMode.MarkdownV2, keyboard,
            BusinessConnectionId, CancellationToken);
    }
}