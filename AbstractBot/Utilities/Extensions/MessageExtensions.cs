using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Utilities.Extensions;

[PublicAPI]
public static class MessageExtensions
{
    public static long GetSenderId(this Message message)
    {
        if (message.SenderChat is not null)
        {
            return message.SenderChat.Id;
        }
        User user = message.From.Denull(nameof(message.From));
        return user.Id;
    }
}