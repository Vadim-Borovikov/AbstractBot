using Telegram.Bot.Exceptions;

namespace AbstractBot.Utilities;

internal static class ErrorHelper
{
    public static bool IsChatNotFoundError(ApiRequestException ex)
    {
        return (ex.ErrorCode == ErrorCodeBadRequest) && (ex.Message == ErrorChatNotFound);
    }

    private const int ErrorCodeBadRequest = 400;
    private const string ErrorChatNotFound = "Bad Request: chat not found";
}