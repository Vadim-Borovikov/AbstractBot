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

    public MessageTemplateImage(MessageTemplate prototype, string imagePath) : base(prototype)
    {
        ImagePath = imagePath;
    }

    public MessageTemplateImage(MessageTemplateImage prototype) : base(prototype)
    {
        ImagePath = prototype.ImagePath;
        HasSpoiler = prototype.HasSpoiler;
    }

    public override MessageTemplateImage Format(params object?[] args)
    {
        return new MessageTemplateImage(this)
        {
            TextJoined = FormatText(args)
        };
    }

    protected override Task<Message> SendAsync(BotBasic bot, Chat chat, string text)
    {
        return bot.SendPhotoAsync(chat, ImagePath, KeyboardProvider, MessageThreadId, text, ParseMode.MarkdownV2,
            Entities, HasSpoiler, DisableNotification, ProtectContent, ReplyToMessageId, AllowSendingWithoutReply,
            CancellationToken);
    }
}