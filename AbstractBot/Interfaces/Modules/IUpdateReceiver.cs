using System.Collections.Generic;
using AbstractBot.Interfaces.Operations;
using GryphonUtilities.Logging;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface IUpdateReceiver
{
    Logger Logger { get; }

    List<IOperation> Operations { get; }

    void Update(Update update);
}