using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Extensions;

[PublicAPI]
public static class DictionaryExtensions
{
    public static void AddAll<TKey, TValue, TTargetValue>(this Dictionary<TKey, TTargetValue> tagret,
        Dictionary<TKey, TValue> source)
        where TKey : notnull
        where TValue : class, TTargetValue
    {
        foreach (TKey key in source.Keys)
        {
            tagret[key] = source[key];
        }
    }

    public static Dictionary<TKey, TTargetValue> FilterByValueType<TKey, TValue, TTargetValue>(
        this Dictionary<TKey, TValue> dict)
        where TKey : notnull
        where TTargetValue : class, TValue
    {
        Dictionary<TKey, TTargetValue> result = new();
        foreach (TKey key in dict.Keys)
        {
            if (dict[key] is TTargetValue val)
            {
                result.Add(key, val);
            }
        }

        return result;
    }
}