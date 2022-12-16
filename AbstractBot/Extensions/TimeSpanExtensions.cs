using JetBrains.Annotations;
using System;

namespace AbstractBot.Extensions;

[PublicAPI]
public static class TimeSpanExtensions
{
    public static TimeSpan? Max(TimeSpan? left, TimeSpan? right)
    {
        if (left is null)
        {
            return right;
        }

        return left.Value.CompareTo(right) switch
        {
            0  => left,
            1  => left,
            -1 => right,
            _  => throw new InvalidOperationException()
        };
    }
}