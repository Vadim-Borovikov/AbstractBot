using System.Collections.Generic;
using AbstractBot.Interfaces;
using AbstractBot.Models;
using JetBrains.Annotations;

namespace AbstractBot.Modules;

[PublicAPI]
public class Accesses : IAccesses
{
    public IEnumerable<long> Ids => _accesses.Keys;
    public Accesses(Dictionary<long, AccessData> accesses) => _accesses = accesses;
    public void AddOrUpdate(long id, AccessData data) => _accesses[id] = data;
    public AccessData GetAccess(long id) => _accesses.ContainsKey(id) ? _accesses[id] : AccessData.Default;

    private readonly Dictionary<long, AccessData> _accesses;
}