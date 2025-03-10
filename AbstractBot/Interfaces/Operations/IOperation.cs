using System;
using System.Threading.Tasks;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
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

    Task<ExecutionResult> TryExecuteAsync(Message message, User from, string? callbackQueryData);

    MessageTemplateText? GetHelpDescriptionFor(User user);
}