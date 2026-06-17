using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateRichText : MessageTemplate
{
    public MessageTemplateRichText(string text) : base(text) { }
    public MessageTemplateRichText(MessageTemplate prototype) : base(prototype) { }

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

    private InputRichMessage GetRichMessage() => new() { Markdown = Text };
}