using AbstractBot.Models.Operations.Commands;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Operations.Commands;

[PublicAPI]
public interface ICommand : IOperation
{
    public BotCommandExtended BotCommandExtended { get; }
}