using AbstractBot.Interfaces.Modules;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Models.MessageTemplates;

[PublicAPI]
public class MessageTemplateImageInputFile : MessageTemplateImage
{
    public InputFile InputFile { get; init; }

    public MessageTemplateImageInputFile(string text, InputFile inputFile) : base(text) => InputFile = inputFile;
    public MessageTemplateImageInputFile(MessageTemplate prototype, InputFile inputFile) : base(prototype)
    {
        InputFile = inputFile;
    }
    public MessageTemplateImageInputFile(MessageTemplateImageInputFile prototype) : base(prototype)
    {
        InputFile = prototype.InputFile;
    }

    public override Task<Message> SendAsync(IUpdateSender updateSender, Chat chat)
    {
        return updateSender.SendPhotoAsync(chat, InputFile, KeyboardProvider, Text, ParseMode.MarkdownV2,
            ReplyParameters, MessageThreadId, Entities, ShowCaptionAboveMedia, HasSpoiler, DisableNotification,
            ProtectContent, MessageEffectId, BusinessConnectionId, AllowPaidBroadcast, DirectMessagesTopicId,
            SuggestedPostParameters, ReceiverUserId, CallbackQueryId, CancellationToken);
    }

    public Task<Message> EditMessageMediaWithSelfAsync(IUpdateSender updateSender, Chat chat, int messageId)
    {
        InlineKeyboardMarkup? keyboard = KeyboardProvider?.Keyboard as InlineKeyboardMarkup;
        return updateSender.EditMessageMediaAsync(chat, messageId, InputFile, Text, ParseMode.MarkdownV2, keyboard,
            BusinessConnectionId, CancellationToken);
    }
}