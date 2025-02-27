using AbstractBot.Interfaces.Modules.Config;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Interfaces.Modules;

[PublicAPI]
public interface ITextsProvider<out TTexts>
    where TTexts : ITexts
{
    public TTexts GetTextsFor(User user);
    public TTexts GetDefaultTexts();
}