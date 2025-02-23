using System.Collections.Generic;
using JetBrains.Annotations;

namespace AbstractBot.Legacy.Extensions;

[PublicAPI]
public static class DictionaryExtensions
{
    public static void AddAll<TKey, TDerivedValue, TBaseValue>(this Dictionary<TKey, TBaseValue> tagret,
        Dictionary<TKey, TDerivedValue> source)
        where TKey : notnull
        where TBaseValue : class
        where TDerivedValue : TBaseValue
    {
        foreach (TKey key in source.Keys)
        {
            tagret[key] = source[key];
        }
    }

    public static Dictionary<TKey, TDerivedValue> FilterByValueType<TKey, TBaseValue, TDerivedValue>(
        this Dictionary<TKey, TBaseValue> dict)
        where TKey : notnull
        where TBaseValue : class
        where TDerivedValue : TBaseValue
    {
        Dictionary<TKey, TDerivedValue> result = new();
        foreach (TKey key in dict.Keys)
        {
            if (dict[key] is TDerivedValue val)
            {
                result.Add(key, val);
            }
        }

        return result;
    }
}