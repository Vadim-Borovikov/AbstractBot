using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Utils
    {
        public static DateTime Now() => TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _timeZoneInfo);

        internal static Task<Message> SendStickerAsync(this ITelegramBotClient client, Message message,
            InputOnlineFile sticker)
        {
            return client.SendStickerAsync(message.Chat, sticker, replyToMessageId: message.MessageId);
        }

        internal static void SetupTimeZoneInfo(string id) => _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(id);

        private static TimeZoneInfo _timeZoneInfo;
    }
}
