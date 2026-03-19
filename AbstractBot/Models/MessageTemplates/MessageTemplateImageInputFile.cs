using System.Threading.Tasks;
using JetBrains.Annotations;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using AbstractBot.Interfaces.Modules;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateImageInputFile : MessageTemplateImage
{
    public InputFile InputFile { get; init; } = null!;

    public MessageTemplateImageInputFile() { }

    public MessageTemplateImageInputFile(string text, InputFile inputFile, bool markdownV2 = false)
        : base(text, markdownV2)
    {
        InputFile = inputFile;
    }

    public MessageTemplateImageInputFile(MessageTemplate prototype, InputFile inputFile) : base(prototype)
    {
        InputFile = inputFile;
    }

    public MessageTemplateImageInputFile(MessageTemplateImage prototype, InputFile inputFile) : base(prototype)
    {
        InputFile = inputFile;
    }

    public MessageTemplateImageInputFile(MessageTemplateImageInputFile prototype) : base(prototype)
    {
        InputFile = prototype.InputFile;
    }

    public override MessageTemplateImageInputFile Format(params object?[] args)
    {
        MessageTemplateFormatInfo info = PrepareFormat(args);

        return new MessageTemplateImageInputFile(this)
        {
            MarkdownV2 = info.MarkdownV2,
            TextJoined = info.Text
        };
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendPhotoAsync(chat, InputFile, KeyboardProvider, TextJoined, ParseMode,
            ReplyParameters, MessageThreadId, Entities, ShowCaptionAboveMedia, HasSpoiler, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId,
            SuggestedPostParameters, CancellationToken);
    }

    public Task<Message> EditMessageMediaWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageMediaAsync(chat, messageId, InputFile, TextJoined, ParseMode, keyboard,
            BusinessConnectionId, CancellationToken);
    }
}