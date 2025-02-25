using System;
using AbstractBot.Interfaces.Modules.Servicies;
using GryphonUtilities;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ILogging : IService
{
    Logger Logger { get; }

    void LogUpdateRefused(Chat chat, Enum type, int? messageId = null, string? data = null);

    void LogUpdate(Chat chat, Enum type, int? messageId = null, string? data = null);
}