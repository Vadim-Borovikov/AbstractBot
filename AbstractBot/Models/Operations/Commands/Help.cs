using AbstractBot.Interfaces.Modules;
using AbstractBot.Interfaces.Modules.Config;
using AbstractBot.Models.MessageTemplates;
using AbstractBot.Utilities.Extensions;
using GryphonUtilities.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace AbstractBot.Models.Operations.Commands;

[PublicAPI]
public class Help : Command
{
    public Help(IAccesses accesses, IUpdateSender updateSender, IUpdateReceiver updateReceiver,
        ITextsProvider<ITexts> textsProvider, string selfUsername)
        : base(accesses, updateSender, "help", textsProvider, selfUsername)
    {
        _accesses = accesses;
        _updateReceiver = updateReceiver;
        _textsProvider = textsProvider;
    }

    protected virtual MessageTemplate GetHelpTemplate(ITexts texts, string descriptions)
    {
        string text = texts.HelpFormat is null ? descriptions : texts.HelpFormat.Format(descriptions);
        return new MessageTemplateText(text);
    }

    protected override Task ExecuteAsync(Message message, User sender)
    {
        ITexts texts = _textsProvider.GetTextsFor(sender.Id);

        AccessData access = _accesses.GetAccess(sender.Id);

        IEnumerable<string> descriptions = _updateReceiver.Operations
                                                          .Where(o => access.IsSufficientAgainst(o.AccessRequired))
                                                          .Select(o => o.GetHelpOperationInfoFor(sender.Id))
                                                          .SkipNulls()
                                                          .OrderBy(info => info.Index)
                                                          .Select(info => info.Description);
        MessageTemplate template = GetHelpTemplate(texts, descriptions.JoinLines().Escape());
        return template.SendAsync(UpdateSender, message.Chat);
    }

    private readonly IAccesses _accesses;
    private readonly IUpdateReceiver _updateReceiver;
    private readonly ITextsProvider<ITexts> _textsProvider;
}