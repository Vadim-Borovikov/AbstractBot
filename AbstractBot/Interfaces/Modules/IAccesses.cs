using System.Collections.Generic;
using AbstractBot.Models;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface IAccesses
{
    IEnumerable<long> Ids { get; }
    AccessData GetAccess(long id);
    void AddOrUpdate(long id, AccessData data);
    void AddOrUpdate(Dictionary<long, AccessData> accesses)
    {
        foreach (long id in accesses.Keys)
        {
            AddOrUpdate(id, accesses[id]);
        }
    }
}