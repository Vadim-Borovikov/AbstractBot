using System.Collections.Generic;
using System.Linq;
using AbstractBot.Bots;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace AbstractBot.Configs.MessageTemplates;

[PublicAPI]
public class MessageTemplateText : MessageTemplate
{
    public LinkPreviewOptions? LinkPreviewOptions;

    public MessageTemplateText() { }

    public MessageTemplateText(string text, bool markdownV2 = false) : base(text, markdownV2) { }

    public MessageTemplateText(MessageTemplate prototype) : base(prototype) { }

    public MessageTemplateText(MessageTemplateText prototype) : base(prototype)
    {
        LinkPreviewOptions = prototype.LinkPreviewOptions;
    }

    public static MessageTemplateText JoinTexts(IList<MessageTemplateText> elements)
    {
        bool shouldEscape = elements.Any(e => e.MarkdownV2);
        IEnumerable<string> lines = elements.Select(e => shouldEscape ? e.EscapeIfNeeded() : e.TextJoined);
        return new MessageTemplateText
        {
            TextJoined = GryphonUtilities.Helpers.Text.JoinLines(lines),
            MarkdownV2 = shouldEscape
        };
    }

    public override MessageTemplateText Format(params object?[] args)
    {
        return new MessageTemplateText(this)
        {
            TextJoined = FormatText(args)
        };
    }

    protected override Task<Message> SendAsync(BotBasic bot, Chat chat, string text)
    {
        return bot.SendTextMessageAsync(chat, text, KeyboardProvider, ParseMode.MarkdownV2, ReplyParameters,
            LinkPreviewOptions, MessageThreadId, Entities, DisableNotification, ProtectContent, MessageEffectId,
            BusinessConnectionId, AllowPaidBroadcast, CancellationToken);
    }
}