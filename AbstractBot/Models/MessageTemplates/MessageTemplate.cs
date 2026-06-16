using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using AbstractBot.Utilities.Extensions;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplate
{
    public IEnumerable<string> Text
    {
        get => Enumerable.Empty<string>();
        init
        {
            TextJoined = string.Join("\n", value);
        }
    }

    public bool Escaped { get; init; }

    public KeyboardProvider? KeyboardProvider;
    public int? MessageThreadId;
    public bool DisableNotification;
    public bool ProtectContent;
    public ReplyParameters? ReplyParameters;
    public string? MessageEffectId;
    public string? BusinessConnectionId;
    public bool AllowPaidBroadcast;
    public long? DirectMessagesTopicId;
    public SuggestedPostParameters? SuggestedPostParameters;
    public CancellationToken CancellationToken;

    protected MessageTemplate() { }

    protected MessageTemplate(string text, bool escaped = false)
    {
        TextJoined = text;
        Escaped = escaped;
    }

    protected MessageTemplate(MessageTemplate prototype)
    {
        TextJoined = prototype.TextJoined;
        Escaped = prototype.Escaped;
        KeyboardProvider = prototype.KeyboardProvider;
        MessageThreadId = prototype.MessageThreadId;
        DisableNotification = prototype.DisableNotification;
        ProtectContent = prototype.ProtectContent;
        ReplyParameters = prototype.ReplyParameters;
        CancellationToken = prototype.CancellationToken;
    }

    protected string TextJoined { get; init; } = null!;

    protected string EscapeIfNeeded() => Escaped ? TextJoined : TextJoined.Escape();

    public abstract Task<Message> SendAsync(IUpdateSender updateSender, Chat chat);

    public abstract MessageTemplate Format(params object?[] args);

    protected MessageTemplateFormatInfo PrepareFormat(params object?[] args)
    {
        bool escaped = Escaped;
        string text = TextJoined;

        // ReSharper disable once MergeIntoPattern
        if (!escaped && args.Any(a => a is MessageTemplate mt && mt.Escaped))
        {
            escaped = true;
            text = text.Escape(false);
        }

        args = args.Select(a => escaped ? EscapeIfNeeded(a) : ExtractText(a)).ToArray();
        text = string.Format(text, args);

        return new MessageTemplateFormatInfo(escaped, text);
    }

    private static object? EscapeIfNeeded(object? o)
    {
        return o is MessageTemplate mt ? mt.EscapeIfNeeded() : o?.ToString()?.Escape();
    }
    private static object? ExtractText(object? o) => o is MessageTemplate mt ? mt.TextJoined : o;
}