using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Modules.TextProviders;

[PublicAPI]
public class Common<TTexts> : ITextsProvider<TTexts>
    where TTexts : ITexts
{
    public readonly TTexts Texts;

    public Common(TTexts texts) => Texts = texts;

    public TTexts GetTextsFor(User user) => Texts;
    public TTexts GetDefaultTexts() => Texts;
}