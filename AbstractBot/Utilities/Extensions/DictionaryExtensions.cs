using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using AbstractBot.Models;

namespace AbstractBot.Utilities.Extensions;

[PublicAPI]
public static class DictionaryExtensions
{
    public static Dictionary<long, AccessData> ToAccessDatasDictionary(this Dictionary<long, int> accesses)
    {
        return accesses.Count > 0
            ? accesses.ToDictionary(p => p.Key, p => new AccessData(p.Value))
            : new Dictionary<long, AccessData>();
    }
}