using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateFile : MessageTemplate
{
    public string FilePath { get; init; } = null!;

    public InputFile? Thumbnail;
    public bool DisableContentTypeDetection;

    public MessageTemplateFile() { }

    public MessageTemplateFile(string text, string filePath, bool markdownV2 = false) : base(text, markdownV2)
    {
        FilePath = filePath;
    }

    public MessageTemplateFile(MessageTemplate prototype, string filePath) : base(prototype)
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
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateFile(this)
        {
            MarkdownV2 = info.MarkdownV2,
            TextJoined = info.Text
        };
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendDocumentAsync(chat, FilePath, KeyboardProvider, TextJoined, ParseMode,
            ReplyParameters, Thumbnail, MessageThreadId, Entities, DisableContentTypeDetection, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, CancellationToken);
    }
}