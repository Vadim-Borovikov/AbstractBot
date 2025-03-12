using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands.Start;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands.Start;

[PublicAPI]
public sealed class Greeter: IGreeter
{
    public Greeter(IUpdateSender updateSender, ITextsProvider<ITexts> textsProvider)
    {
        _updateSender = updateSender;
        _textsProvider = textsProvider;
    }

    public Task Greet(Message message, User from)
    {
        ITexts texts = _textsProvider.GetTextsFor(from.Id);
        return texts.StartFormat.SendAsync(_updateSender, message.Chat);
    }

    private readonly IUpdateSender _updateSender;
    private readonly ITextsProvider<ITexts> _textsProvider;
}