using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateRichText : MessageTemplate
{
    public readonly IDictionary<string, IInputRichMedia>? Media;

    public MessageTemplateRichText(string text, IDictionary<string, IInputRichMedia>? media = null) : base(text)
    {
        Media = media;
    }
    public MessageTemplateRichText(string text, IDictionary<string, InputFile>? media)
        : this(text, media?.ToDictionary(p => p.Key, p => new InputMediaPhoto(p.Value) as IInputRichMedia)) { }

    public MessageTemplateRichText(MessageTemplate prototype, IDictionary<string, IInputRichMedia>? media = null) :
        base(prototype)
    {
        Media = media;
    }
    public MessageTemplateRichText(MessageTemplate prototype, IDictionary<string, InputFile>? media)
        : this(prototype, media?.ToDictionary(p => p.Key, p => new InputMediaPhoto(p.Value) as IInputRichMedia)) { }

    public Task<Message> EditMessageWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageTextAsync(chat, messageId, string.Empty, ParseMode.None, keyboard, null,
            null, GetRichMessage(), BusinessConnectionId, CancellationToken);
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendRichMessageAsync(chat, GetRichMessage(), KeyboardProvider, ReplyParameters,
            MessageThreadId, DisableNotification, ProtectContent, MessageEffectId, BusinessConnectionId,
            AllowPaidBroadcast, DirectMessagesTopicId, SuggestedPostParameters, CancellationToken);
    }

    private InputRichMessage GetRichMessage()
    {
        return new InputRichMessage
        {
            Markdown = Text,
            Media = Media?.Select(p => new InputRichMessageMedia { Id = p.Key, Media = p.Value})
        };
    }
}