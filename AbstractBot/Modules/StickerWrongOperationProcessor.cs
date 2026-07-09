using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Operations;
using JetBrains.Annotations;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Modules;

[PublicAPI]
public class StickerWrongOperationProcessor : IWrongOperationProcessor
{
    public StickerWrongOperationProcessor(InputFileId dontUnderstandSticker, InputFileId forbiddenSticker)
    {
        _dontUnderstandSticker = dontUnderstandSticker;
        _forbiddenSticker = forbiddenSticker;
    }

    public Task ProcessUnclearOperationAsync(IUpdateSender sender, Message message, User? _)
    {
        return SendStickerAsync(sender, message, _dontUnderstandSticker);
    }

    public Task ProcessInsufficientAccessAsync(IUpdateSender sender, Message message, User _, IOperation __)
    {
        return SendStickerAsync(sender, message, _forbiddenSticker);
    }

    public Task ProcessExpiredAccessAsync(IUpdateSender sender, Message message, User _, IOperation __)
    {
        return ProcessInsufficientAccessAsync(sender, message, _, __);
    }

    private static Task SendStickerAsync(IUpdateSender sender, Message message, InputFile sticker)
    {
        if (message.Chat.Type == ChatType.Channel)
        {
            return Task.CompletedTask;
        }

        Chat chat = message.Chat;
        ReplyParameters rp = new() { MessageId = message.MessageId };
        return sender.SendStickerAsync(chat, sticker, rp);
    }

    private readonly InputFileId _dontUnderstandSticker;
    private readonly InputFileId _forbiddenSticker;
}
