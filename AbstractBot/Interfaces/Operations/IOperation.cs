using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Operations;

[PublicAPI]
public interface IOperation
{
    enum ExecutionResult
    {
        UnsuitableOperation,
        AccessInsufficent,
        AccessExpired,
        Success
    }

    Enum? AccessRequired { get; }

    bool EnabledInGroups { get; }
    bool EnabledInChannels { get; }

    Task<ExecutionResult> TryExecuteAsync(Message message, User? from, CallbackQuery? callbackQuery);

    string? GetHelpDescriptionFor(long userId);
}