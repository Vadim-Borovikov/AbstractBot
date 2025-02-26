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

    public static void ReplaceWith<TSource, TTarget>(this Dictionary<long, TTarget> target,
        Dictionary<long, TSource> source, Func<TSource, TTarget?> convert)
        where TTarget : class
    {
        target.Clear();

        foreach (long id in source.Keys)
        {
            TTarget? t = convert(source[id]);
            if (t is not null)
            {
                target[id] = t;
            }
        }
    }

    /*public static void AddAll<TKey, TDerivedValue, TBaseValue>(this Dictionary<TKey, TBaseValue> tagret,
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
    }*/
}