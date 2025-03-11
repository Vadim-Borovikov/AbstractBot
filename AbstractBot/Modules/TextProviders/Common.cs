using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;

namespace AbstractBot.Modules.TextProviders;

[PublicAPI]
public class Common<TTexts> : ITextsProvider<TTexts>
    where TTexts : ITexts
{
    public Common(TTexts texts) => _texts = texts;

    public TTexts GetTextsFor(long userId) => _texts;
    public TTexts GetDefaultTexts() => _texts;

    private readonly TTexts _texts;
}