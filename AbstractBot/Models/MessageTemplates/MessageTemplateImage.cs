using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplateImage : MessageTemplate
{
    public bool ShowCaptionAboveMedia;
    public bool HasSpoiler;

    protected MessageTemplateImage() { }

    protected MessageTemplateImage(string text, bool markdownV2 = false) : base(text, markdownV2) { }

    protected MessageTemplateImage(MessageTemplate prototype) : base(prototype) { }

    protected MessageTemplateImage(MessageTemplateImage prototype) : base(prototype)
    {
        ShowCaptionAboveMedia = prototype.ShowCaptionAboveMedia;
        HasSpoiler = prototype.HasSpoiler;
    }

    public Task<Message> EditMessageTextWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, TextJoined, ParseMode, keyboard, null, Entities,
            BusinessConnectionId, CancellationToken);
    }

    public Task<Message> EditMessageCaptionWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageCaptionAsync(chat, messageId, TextJoined, ParseMode, keyboard, Entities,
            ShowCaptionAboveMedia, BusinessConnectionId, CancellationToken);
    }
}