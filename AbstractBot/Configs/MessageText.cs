using System;
using System.Collections.Generic;
using System.Linq;
using AbstractBot.Bots;
using System.Threading.Tasks;
using System.Threading;
using AbstractBot.Extensions;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

// ReSharper disable NullableWarningSuppressionIsUsed

namespace AbstractBot.Configs;

[PublicAPI]
public class MessageText
{
    public List<string> Text { get; init; } = null!;

    public bool MarkdownV2 { get; init; }

    public MessageText() { }

    public MessageText(string text, bool markdownV2 = false)
    {
        Text = text.WrapWithList();
        MarkdownV2 = markdownV2;
    }

    private string Join() => GryphonUtilities.Text.JoinLines(Text);

    public string EscapeIfNeeded() => MarkdownV2 ? Join() : Join().Escape();

    public MessageText Format(params object?[] args)
    {
        string text;
        if (MarkdownV2)
        {
            // Will not escape whole thing!
            args =
                args.Select(a => a is MessageText mt ? mt.EscapeIfNeeded() : (object?) a?.ToString()?.Escape())
                    .ToArray();
            text = GryphonUtilities.Text.FormatLines(Text, args);
        }
        else
        {
            // Will escape the whole thing!
            foreach (object? a in args)
            {
                // ReSharper disable once MergeIntoPattern
                if (a is MessageText mt && mt.MarkdownV2)
                {
                    throw new InvalidOperationException(
                        $"Can't put MarkdownV2 parameter {mt.Join()} into non-MarkdownV2 format {Join()}");
                }
            }
            args = args.Select(a => a is MessageText mt ? mt.Join() : a).ToArray();
            text = GryphonUtilities.Text.FormatLines(Text, args);
        }

        return new MessageText(text, MarkdownV2);
    }

    public Task<Message> SendAsync(BotBasic bot, Chat chat, KeyboardProvider? keyboardProvider = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? entities = null, bool? disableWebPagePreview = null,
        bool? disableNotification = null, bool? protectContent = null, int? replyToMessageId = null,
        bool? allowSendingWithoutReply = null, CancellationToken cancellationToken = default)
    {
        return bot.SendTextMessageAsync(chat, EscapeIfNeeded(), keyboardProvider, ParseMode.MarkdownV2,
            messageThreadId, entities, disableWebPagePreview, disableNotification, protectContent, replyToMessageId,
            allowSendingWithoutReply, cancellationToken);
    }
}