using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateRichText : MessageTemplate
{
    public MessageTemplateRichText() { }

    public MessageTemplateRichText(string text, bool escaped = false) : base(text, escaped) { }

    public MessageTemplateRichText(MessageTemplate prototype) : base(prototype) { }

    public static MessageTemplateRichText JoinTexts(IReadOnlyCollection<MessageTemplateRichText> elements,
        string separator = "\n")
    {
        bool shouldEscape = elements.Any(e => e.Escaped);
        IEnumerable<string> parts = elements.Select(e => shouldEscape ? e.EscapeIfNeeded() : e.TextJoined);
        return new MessageTemplateRichText
        {
            TextJoined = string.Join(separator, parts),
            Escaped = shouldEscape
        };
    }

    public override MessageTemplateRichText Format(params object?[] args)
    {
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateRichText(this)
        {
            Escaped = info.Escaped,
            TextJoined = info.Text
        };
    }

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

    private InputRichMessage GetRichMessage() => new() { Markdown = EscapeIfNeeded() };
}