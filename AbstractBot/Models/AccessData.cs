using GryphonUtilities.Time;
using System;
using JetBrains.Annotations;

namespace AbstractBot.Models;

[PublicAPI]
public sealed class AccessData
{
    public enum Status
    {
        Sufficient,
        Insufficient,
        Expired
    }

    public AccessData(Enum access, DateTimeFull? until = null) : this(ToInt(access), until) { }

    internal AccessData(int access, DateTimeFull? until = null)
    {
        _access = access;
        _until = until;
    }

    internal static readonly AccessData Default = new(DefaultAccess);

    public bool IsSufficientAgainst(Enum? required)
    {
        return IsSufficientAgainst(required is null ? DefaultAccess : ToInt(required));
    }

    internal Status CheckAgainst(Enum? required) => CheckAgainst(required is null ? DefaultAccess : ToInt(required));

    private bool IsSufficientAgainst(int required) => CheckAgainst(required) == Status.Sufficient;

    private Status CheckAgainst(int required)
    {
        if (!IsSufficient(_access, required))
        {
            return Status.Insufficient;
        }

        if (_until.HasValue)
        {
            DateTimeFull now = DateTimeFull.CreateNow(_until.Value.TimeZoneInfo);
            if (_until.Value < now)
            {
                return Status.Expired;
            }
        }

        return Status.Sufficient;
    }

    private static bool IsSufficient(int provided, int required) => (provided & required) != 0;

    private static int ToInt(Enum access) => Convert.ToInt32(access);

    private const int DefaultAccess = 1;

    private readonly int _access;
    private readonly DateTimeFull? _until;
}