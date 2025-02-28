using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules.TextProviders;

[PublicAPI]
public class Common<TTexts> : ITextsProvider<TTexts>
    where TTexts : ITexts
{
    public Common(TTexts texts) => _texts = texts;

    public TTexts GetTextsFor(User user) => _texts;
    public TTexts GetDefaultTexts() => _texts;

    private readonly TTexts _texts;
}