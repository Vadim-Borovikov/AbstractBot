using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Models.MessageTemplates;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public sealed class Help : Command
{
    public Help(IAccesses accesses, IUpdateSender updateSender, IUpdateReceiver updateReceiver,
        ITextsProvider<ITexts> textsProvider, string selfUsername)
        : base(accesses, updateSender, "help", textsProvider, selfUsername)
    {
        _accesses = accesses;
        _updateReceiver = updateReceiver;
        _textsProvider = textsProvider;
    }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        ITexts texts = _textsProvider.GetTextsFor(sender);

        AccessData access = _accesses.GetAccess(sender.Id);

        List<MessageTemplateText> descriptions =
            _updateReceiver.Operations
                           .Where(o => access.IsSufficientAgainst(o.AccessRequired))
                           .Select(o => o.GetHelpDescriptionFor(sender))
                           .SkipNulls()
                           .ToList();

        MessageTemplateText template = MessageTemplateText.JoinTexts(descriptions);
        MessageTemplateText? format = texts.HelpFormat;
        if (format is not null)
        {
            template = format.Format(template);
        }

        return template.SendAsync(UpdateSender, message.Chat);
    }

    private readonly IAccesses _accesses;
    private readonly IUpdateReceiver _updateReceiver;
    private readonly ITextsProvider<ITexts> _textsProvider;
}