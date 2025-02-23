using System.Collections.Generic;
using AbstractBot.Models;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces;

[PublicAPI]
public interface IAccesses
{
    public IEnumerable<long> Ids { get; }
    public AccessData GetAccess(long id);
    void AddOrUpdate(long id, AccessData data);
    void AddOrUpdate(Dictionary<long, AccessData> accesses)
    {
        foreach (long id in accesses.Keys)
        {
            AddOrUpdate(id, accesses[id]);
        }
    }
}