using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AbstractBot.Bots;
using System.Threading.Tasks;
using AbstractBot.Extensions;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Configs.MessageTemplates;

[PublicAPI]
public abstract class MessageTemplate
{
    public List<string> Text { get; init; } = null!;

    public bool MarkdownV2 { get; init; }

    public KeyboardProvider? KeyboardProvider;
    public int? MessageThreadId;
    public IEnumerable<MessageEntity>? Entities;
    public bool? DisableNotification;
    public bool? ProtectContent;
    public int? ReplyToMessageId;
    public bool? AllowSendingWithoutReply;
    public CancellationToken CancellationToken;

    protected MessageTemplate() { }

    protected MessageTemplate(string text, bool markdownV2 = false)
    {
        Text = text.WrapWithList();
        MarkdownV2 = markdownV2;
    }

    private string Join() => GryphonUtilities.Helpers.Text.JoinLines(Text);

    public string EscapeIfNeeded() => MarkdownV2 ? Join() : Join().Escape();

    protected string FormatText(params object?[] args)
    {
        if (MarkdownV2)
        {
            // Will not escape whole thing!
            args = args.Select(a => a is MessageTemplateText mtt
                ? mtt.EscapeIfNeeded()
                : (object?)a?.ToString()?.Escape()).ToArray();
            return GryphonUtilities.Helpers.Text.FormatLines(Text, args);
        }

        // Will escape the whole thing!
        foreach (object? a in args)
        {
            // ReSharper disable once MergeIntoPattern
            if (a is MessageTemplateText mtt && mtt.MarkdownV2)
            {
                throw new InvalidOperationException(
                    $"Can't put MarkdownV2 parameter {mtt.Join()} into non-MarkdownV2 format {Join()}");
            }
        }
        args = args.Select(a => a is MessageTemplateText mtt ? mtt.Join() : a).ToArray();
        return GryphonUtilities.Helpers.Text.FormatLines(Text, args);
    }

    public Task<Message> SendAsync(BotBasic bot, Chat chat) => SendAsync(bot, chat, EscapeIfNeeded());

    protected abstract Task<Message> SendAsync(BotBasic bot, Chat chat, string text);
}