using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplateImage : MessageTemplateMarkdownV2
{
    public bool ShowCaptionAboveMedia;
    public bool HasSpoiler;

    protected MessageTemplateImage(string text) : base(text) { }
    protected MessageTemplateImage(MessageTemplate prototype) : base(prototype) { }
    protected MessageTemplateImage(MessageTemplateImage prototype) : base(prototype)
    {
        ShowCaptionAboveMedia = prototype.ShowCaptionAboveMedia;
        HasSpoiler = prototype.HasSpoiler;
    }

    public Task<Message> EditMessageTextWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, Text, ParseMode.MarkdownV2, keyboard, null, Entities,
            null, BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageCaptionWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageCaptionAsync(chat, messageId, Text, ParseMode.MarkdownV2, keyboard, Entities,
            ShowCaptionAboveMedia, BusinessConnectionId, CancellationToken);
    }
}