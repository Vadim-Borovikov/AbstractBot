using System;
using System.Collections.Generic;
using System.Linq;
using AbstractBot.Models;
using JetBrains.Annotations;

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

    public static Dictionary<long, T2> Convert<T1, T2>(this Dictionary<long, T1> data, Func<T1, T2?> convert)
    {
        Dictionary<long, T2> result = new();
        foreach (long key in data.Keys)
        {
            T2? finalData = convert(data[key]);
            if (finalData is not null)
            {
                result[key] = finalData;
            }
        }
        return result;
    }
}