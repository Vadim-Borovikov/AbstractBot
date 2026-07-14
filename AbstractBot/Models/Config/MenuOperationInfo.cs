using JetBrains.Annotations;

namespace AbstractBot.Models.Config;

[PublicAPI]
public readonly record struct MenuOperationInfo(int Index, string Description);