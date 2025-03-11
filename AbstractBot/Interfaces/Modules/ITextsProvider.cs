using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ITextsProvider<out TTexts>
    where TTexts : ITexts
{
    public TTexts GetTextsFor(long userId);
    public TTexts GetDefaultTexts();
}