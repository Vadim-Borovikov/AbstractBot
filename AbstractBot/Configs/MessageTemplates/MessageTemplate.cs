using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AbstractBot.Bots;
using System.Threading.Tasks;
using AbstractBot.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;

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

    public string EscapeIfNeeded() => MarkdownV2 ? TextJoined : TextJoined.Escape();

    public Task<Message> SendAsync(BotBasic bot, Chat chat) => SendAsync(bot, chat, EscapeIfNeeded());

    public abstract MessageTemplate Format(params object?[] args);

    protected string FormatText(params object?[] args)
    {
        if (MarkdownV2)
        {
            // Will not escape whole thing!
            args = args.Select(a => a is MessageTemplate mt
                ? mt.EscapeIfNeeded()
                : (object?)a?.ToString()?.Escape()).ToArray();
        }
        else
        {
            // Will escape the whole thing!
            foreach (object? a in args)
            {
                // ReSharper disable once MergeIntoPattern
                if (a is MessageTemplate mtt && mtt.MarkdownV2)
                {
                    throw new InvalidOperationException(
                        $"Can't put MarkdownV2 parameter {mtt.TextJoined} into non-MarkdownV2 format {TextJoined}");
                }
            }
            args = args.Select(a => a is MessageTemplate mt ? mt.TextJoined : a).ToArray();
        }
        return string.Format(TextJoined, args);
    }

    protected abstract Task<Message> SendAsync(BotBasic bot, Chat chat, string text);

    protected string TextJoined { get; init; } = null!;
}