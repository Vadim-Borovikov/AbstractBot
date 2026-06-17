using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateFile : MessageTemplateMarkdownV2
{
    public string FilePath { get; init; }

    public InputFile? Thumbnail;
    public bool DisableContentTypeDetection;

    public MessageTemplateFile(string text, string filePath) : base(text) => FilePath = filePath;
    public MessageTemplateFile(MessageTemplate prototype, string filePath) : base(prototype) => FilePath = filePath;
    public MessageTemplateFile(MessageTemplateFile prototype) : base(prototype)
    {
        FilePath = prototype.FilePath;
        Thumbnail = prototype.Thumbnail;
        DisableContentTypeDetection = prototype.DisableContentTypeDetection;
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendDocumentAsync(chat, FilePath, KeyboardProvider, Text, ParseMode.MarkdownV2,
            ReplyParameters, Thumbnail, MessageThreadId, Entities, DisableContentTypeDetection, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId,
            SuggestedPostParameters, CancellationToken);
    }
}