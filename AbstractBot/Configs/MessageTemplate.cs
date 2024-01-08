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
public class MessageTemplate
{
    public List<string> Text { get; init; } = null!;

    public bool MarkdownV2 { get; init; }

    public string? ImagePath { get; init; }

    public MessageTemplate() { }

    public MessageTemplate(string text,  bool markdownV2 = false, string? imagePath = null)
    {
        Text = text.WrapWithList();
        MarkdownV2 = markdownV2;
        ImagePath = imagePath;
    }

    public static MessageTemplate? JoinTexts(IList<MessageTemplate> elements)
    {
        if (elements.Any(e => !string.IsNullOrWhiteSpace(e.ImagePath)))
        {
            return null;
        }

        bool shouldEscape = elements.Any(e => e.MarkdownV2);
        return new MessageTemplate
        {
            Text = shouldEscape
                ? elements.Select(e => e.EscapeIfNeeded()).ToList()
                : elements.SelectMany(e => e.Text).ToList(),
            MarkdownV2 = shouldEscape
        };
    }

    private string Join() => GryphonUtilities.Helpers.Text.JoinLines(Text);

    public string EscapeIfNeeded() => MarkdownV2 ? Join() : Join().Escape();

    public MessageTemplate Format(params object?[] args)
    {
        string text;
        if (MarkdownV2)
        {
            // Will not escape whole thing!
            args =
                args.Select(a => a is MessageTemplate mt ? mt.EscapeIfNeeded() : (object?) a?.ToString()?.Escape())
                    .ToArray();
            text = GryphonUtilities.Helpers.Text.FormatLines(Text, args);
        }
        else
        {
            // Will escape the whole thing!
            foreach (object? a in args)
            {
                // ReSharper disable once MergeIntoPattern
                if (a is MessageTemplate mt && mt.MarkdownV2)
                {
                    throw new InvalidOperationException(
                        $"Can't put MarkdownV2 parameter {mt.Join()} into non-MarkdownV2 format {Join()}");
                }
            }
            args = args.Select(a => a is MessageTemplate mt ? mt.Join() : a).ToArray();
            text = GryphonUtilities.Helpers.Text.FormatLines(Text, args);
        }

        return new MessageTemplate(text, MarkdownV2, ImagePath);
    }

    public Task<Message> SendAsync(BotBasic bot, Chat chat, KeyboardProvider? keyboardProvider = null,
        int? messageThreadId = null, IEnumerable<MessageEntity>? entities = null, bool? disableNotification = null,
        bool? protectContent = null, int? replyToMessageId = null, bool? allowSendingWithoutReply = null,
        CancellationToken cancellationToken = default, bool? photoHasSpoiler = false,
        bool? textDisableWebPagePreview = null)
    {
        string text = EscapeIfNeeded();

        if (string.IsNullOrWhiteSpace(ImagePath))
        {
            return bot.SendTextMessageAsync(chat, text, keyboardProvider, ParseMode.MarkdownV2, messageThreadId,
                entities, textDisableWebPagePreview, disableNotification, protectContent, replyToMessageId,
                allowSendingWithoutReply, cancellationToken);
        }

        return bot.SendPhotoAsync(chat, ImagePath, keyboardProvider, messageThreadId, text, ParseMode.MarkdownV2,
            entities, photoHasSpoiler, disableNotification, protectContent, replyToMessageId, allowSendingWithoutReply,
            cancellationToken);
    }
}