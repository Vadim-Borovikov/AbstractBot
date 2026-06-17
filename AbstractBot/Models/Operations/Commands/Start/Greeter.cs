using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Interfaces.Operations.Commands.Start;
using AbstractBot.Models.MessageTemplates;
using JetBrains.Annotations;
using System.Threading.Tasks;
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

    public Task GreetAsync(Message message, User from)
    {
        ITexts texts = _textsProvider.GetTextsFor(from.Id);
        MessageTemplateText template = new(texts.StartFormat.ToString());
        return template.SendAsync(_updateSender, message.Chat);
    }

    private readonly IUpdateSender _updateSender;
    private readonly ITextsProvider<ITexts> _textsProvider;
}