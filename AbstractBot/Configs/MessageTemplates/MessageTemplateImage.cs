using AbstractBot.Bots;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace AbstractBot.Configs.MessageTemplates;

[PublicAPI]
public class MessageTemplateImage : MessageTemplate
{
    public string ImagePath { get; init; } = null!;

    public bool? HasSpoiler;

    public MessageTemplateImage() { }

    public MessageTemplateImage(string text, string imagePath, bool markdownV2 = false) : base(text, markdownV2)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImage Format(params object?[] args)
    {
        string text = FormatText(args);
        return new MessageTemplateImage(text, ImagePath, MarkdownV2);
    }

    protected override Task<Message> SendAsync(BotBasic bot, Chat chat, string text)
    {
        return bot.SendPhotoAsync(chat, ImagePath, KeyboardProvider, MessageThreadId, text, ParseMode.MarkdownV2,
            Entities, HasSpoiler, DisableNotification, ProtectContent, ReplyToMessageId, AllowSendingWithoutReply,
            CancellationToken);
    }
}