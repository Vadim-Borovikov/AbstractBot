using AbstractBot.Bots;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Configs.MessageTemplates;

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
        return new MessageTemplateImage(this)
        {
            TextJoined = FormatText(args)
        };
    }

    public override Task<Message> SendAsync(BotBasic bot, Chat chat)
    {
        return bot.SendPhotoAsync(chat, ImagePath, KeyboardProvider, TextJoined, ParseMode, ReplyParameters,
            MessageThreadId, Entities, ShowCaptionAboveMedia, HasSpoiler, DisableNotification, ProtectContent,
            MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, CancellationToken);
    }

    public Task<Message> EditMessageTextWithSelfAsync(BotBasic bot, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return bot.EditMessageTextAsync(chat, messageId, TextJoined, ParseMode, Entities, null, keyboard,
            BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageMediaWithSelfAsync(BotBasic bot, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return bot.EditMessageMediaAsync(chat, messageId, ImagePath, keyboard, BusinessConnectionId,
            CancellationToken);
    }

    public Task<Message> EditMessageCaptionWithSelfAsync(BotBasic bot, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return bot.EditMessageCaptionAsync(chat, messageId, TextJoined, ParseMode, Entities, ShowCaptionAboveMedia,
            keyboard, BusinessConnectionId, CancellationToken);
    }
}