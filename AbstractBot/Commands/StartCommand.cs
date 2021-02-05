using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace AbstractBot.Commands
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class StartCommand : CommandBase
    {
        public override string Name => "start";
        public override string Description => "инструкции и команды";

        protected StartCommand(IDescriptionProvider descriptionProvider) => DescriptionProvider = descriptionProvider;

        public override Task ExecuteAsync(ChatId chatId, ITelegramBotClient client, int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null)
        {
            return client.SendTextMessageAsync(chatId, DescriptionProvider.GetDescription(),
                replyToMessageId: replyToMessageId, replyMarkup: replyMarkup);
        }

        protected readonly IDescriptionProvider DescriptionProvider;
    }
}
