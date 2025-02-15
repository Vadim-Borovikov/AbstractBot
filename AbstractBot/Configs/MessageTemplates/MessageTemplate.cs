using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AbstractBot.Bots;
using System.Threading.Tasks;
using AbstractBot.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Configs.MessageTemplates;

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

    protected string EscapeIfNeeded() => MarkdownV2 ? TextJoined : TextJoined.Escape();

    protected ParseMode ParseMode => MarkdownV2 ? ParseMode.MarkdownV2 : ParseMode.None;

    public abstract Task<Message> SendAsync(BotBasic bot, Chat chat);

    public abstract MessageTemplate Format(params object?[] args);

    private object? EscapeIfNeeded(object? o)
    {
        return o is MessageTemplate mt ? mt.EscapeIfNeeded() : o?.ToString()?.Escape();
    }
    private object? ExtractText(object? o) => o is MessageTemplate mt ? mt.TextJoined : o;

    protected string FormatText(params object?[] args) => FormatText(MarkdownV2, TextJoined, args);

    private string FormatText(bool markdownV2, string text, params object?[] args)
    {
        // ReSharper disable once MergeIntoPattern
        if (!markdownV2 && args.Any(a => a is MessageTemplate mtt && mtt.MarkdownV2))
        {
            markdownV2 = true;
            text = text.Escape(false);
        }

        args = args.Select(a => markdownV2 ? EscapeIfNeeded(a) : ExtractText(a)).ToArray();
        return string.Format(text, args);
    }

    protected string TextJoined { get; init; } = null!;
}