using System;
using JetBrains.Annotations;

namespace AbstractBot.Helpers;

[PublicAPI]
public static class Access
{
    public static T ToEnum<T>(int access) where T : struct, Enum => (T)Enum.ToObject(typeof(T), access);

    public static int ToInt<T>(T access) where T : struct, Enum => Convert.ToInt32(access);

    public static bool IsSufficient<T>(T provided, T required) where T : struct, Enum => provided.HasFlag(required);

    public static bool IsSufficient(int provided, int required) => (provided & required) > 0;
}