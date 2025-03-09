using AbstractBot.Modules;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules.Servicies;

[PublicAPI]
public interface ILogging : IService
{
    LoggerExtended Logger { get; }
}