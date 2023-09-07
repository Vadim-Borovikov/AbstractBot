using System;
using System.Threading.Tasks;
using AbstractBot.Bots;
using AbstractBot.Operations.Infos;
using GryphonUtilities;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AbstractBot.Operations.Commands;

public sealed class Start : CommandSimple
{
    protected override byte Order => 0;

    internal Start(BotBasic bot) : base(bot, "start", bot.Config.Texts.StartCommandDescription) { }

    protected override bool IsInvokingByPayload(Message message, User sender, string payload, out InfoCommand info)
    {
        info = new InfoCommand(payload);
        return true;
    }

    protected override Task ExecuteAsync(InfoCommand info, Message message, User sender)
    {
        return info.Payload == "" ? Greet(message.Chat) : Bot.OnStartCommand(this, message, sender, info.Payload);
    }

    internal async Task Greet(Chat chat)
    {
        await Bot.UpdateCommandsFor(chat.Id);

        string text = $"{Bot.About}";
        if (Bot.Config.Texts.StartPostfixLinesMarkdownV2 is not null)
        {
            text += $"{Environment.NewLine}{Text.JoinLines(Bot.Config.Texts.StartPostfixLinesMarkdownV2)}";
        }
        await Bot.SendTextMessageAsync(chat, text, ParseMode.MarkdownV2);
    }
}