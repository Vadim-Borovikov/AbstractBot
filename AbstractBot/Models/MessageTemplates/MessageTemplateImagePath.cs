using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateImagePath : MessageTemplateImage
{
    public string ImagePath { get; init; } = null!;

    public MessageTemplateImagePath() { }

    public MessageTemplateImagePath(string text, string imagePath, bool markdownV2 = false) : base(text, markdownV2)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImagePath(MessageTemplate prototype, string imagePath) : base(prototype)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImagePath(MessageTemplateImage prototype, string imagePath) : base(prototype)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImagePath(MessageTemplateImagePath prototype) : base(prototype)
    {
        ImagePath = prototype.ImagePath;
    }

    public override MessageTemplateImagePath Format(params object?[] args)
    {
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateImagePath(this)
        {
            MarkdownV2 = info.MarkdownV2,
            TextJoined = info.Text
        };
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendPhotoAsync(chat, ImagePath, KeyboardProvider, TextJoined, ParseMode,
            ReplyParameters, MessageThreadId, Entities, ShowCaptionAboveMedia, HasSpoiler, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId,
            SuggestedPostParameters, CancellationToken);
    }

    public Task<Message> EditMessageMediaWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageMediaAsync(chat, messageId, ImagePath, TextJoined, ParseMode, keyboard,
            BusinessConnectionId, CancellationToken);
    }
}