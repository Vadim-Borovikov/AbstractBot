using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using AbstractBot.Legacy.Bots;
using AbstractBot.Legacy.Extensions;
using AbstractBot.Models;

namespace AbstractBot.Legacy.Configs.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplate
{
    public IEnumerable<string> Text
    {
        get => Enumerable.Empty<string>();
        init
        {
            TextJoined = GryphonUtilities.Helpers.Text.JoinLines(value);
        }
    }

    public bool MarkdownV2 { get; init; }

    public KeyboardProvider? KeyboardProvider;
    public int? MessageThreadId;
    public IEnumerable<MessageEntity>? Entities;
    public bool DisableNotification;
    public bool ProtectContent;
    public ReplyParameters? ReplyParameters;
    public string? MessageEffectId;
    public string? BusinessConnectionId;
    public bool AllowPaidBroadcast;
    public CancellationToken CancellationToken;

    protected MessageTemplate() { }

    protected MessageTemplate(string text, bool markdownV2 = false)
    {
        TextJoined = text;
        MarkdownV2 = markdownV2;
    }

    protected MessageTemplate(MessageTemplate prototype)
    {
        TextJoined = prototype.TextJoined;
        MarkdownV2 = prototype.MarkdownV2;
        KeyboardProvider = prototype.KeyboardProvider;
        MessageThreadId = prototype.MessageThreadId;
        Entities = prototype.Entities;
        DisableNotification = prototype.DisableNotification;
        ProtectContent = prototype.ProtectContent;
        ReplyParameters = prototype.ReplyParameters;
        CancellationToken = prototype.CancellationToken;
    }

    protected string TextJoined { get; init; } = null!;

    protected string EscapeIfNeeded() => MarkdownV2 ? TextJoined : TextJoined.Escape();

    protected ParseMode ParseMode => MarkdownV2 ? ParseMode.MarkdownV2 : ParseMode.None;

    public abstract Task<Message> SendAsync(BotBasic bot, Chat chat);

    public abstract MessageTemplate Format(params object?[] args);

    protected MessageTemplateFormatInfo PrepareFormat(params object?[] args)
    {
        bool markdownV2 = MarkdownV2;
        string text = TextJoined;

        // ReSharper disable once MergeIntoPattern
        if (!markdownV2 && args.Any(a => a is MessageTemplate mt && mt.MarkdownV2))
        {
            markdownV2 = true;
            text = text.Escape(false);
        }

        args = args.Select(a => markdownV2 ? EscapeIfNeeded(a) : ExtractText(a)).ToArray();
        text = string.Format(text, args);

        return new MessageTemplateFormatInfo(markdownV2, text);
    }

    private static object? EscapeIfNeeded(object? o)
    {
        return o is MessageTemplate mt ? mt.EscapeIfNeeded() : o?.ToString()?.Escape();
    }
    private static object? ExtractText(object? o) => o is MessageTemplate mt ? mt.TextJoined : o;
}