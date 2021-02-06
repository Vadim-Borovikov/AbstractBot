using System.Diagnostics.CodeAnalysis;

namespace AbstractBot
{
    [SuppressMessage("ReSharper", "MemberCanBeInternal")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public interface IDescriptionProvider
    {
        string GetDescription();
    }
}
