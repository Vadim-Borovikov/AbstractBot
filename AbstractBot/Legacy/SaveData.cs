using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Legacy;

[PublicAPI]
public class SaveData<TContextData>
    where TContextData : class
{
    public Dictionary<long, TContextData> ContextDatas { get; init; } = new();

    [UsedImplicitly]
    public SaveData() { }
}