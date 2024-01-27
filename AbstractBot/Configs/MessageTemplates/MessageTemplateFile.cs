using AbstractBot.Bots;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;

namespace AbstractBot.Configs.MessageTemplates;

[PublicAPI]
public class MessageTemplateFile : MessageTemplate
{
    public string FilePath { get; init; } = null!;

    public InputFile? Thumbnail;
    public bool? DisableContentTypeDetection;

    public MessageTemplateFile() { }

    public MessageTemplateFile(string text, string filePath, bool markdownV2 = false) : base(text, markdownV2)
    {
        FilePath = filePath;
    }

    public MessageTemplateFile(MessageTemplateFile prototype) : base(prototype)
    {
        FilePath = prototype.FilePath;
        Thumbnail = prototype.Thumbnail;
        DisableContentTypeDetection = prototype.DisableContentTypeDetection;
    }

    public override MessageTemplateFile Format(params object?[] args)
    {
        return new MessageTemplateFile(this)
        {
            TextJoined = FormatText(args)
        };
    }


    protected override Task<Message> SendAsync(BotBasic bot, Chat chat, string text)
    {
        return bot.SendDocumentAsync(chat, FilePath, KeyboardProvider, MessageThreadId, Thumbnail, text,
            ParseMode.MarkdownV2, Entities, DisableContentTypeDetection, DisableNotification, ProtectContent,
            ReplyToMessageId, AllowSendingWithoutReply, CancellationToken);
    }
}