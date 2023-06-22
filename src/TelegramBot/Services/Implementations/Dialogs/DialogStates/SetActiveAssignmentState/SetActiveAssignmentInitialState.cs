using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Interfaces.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.SetActiveAssignmentState
{
    public class SetActiveAssignmentInitialState : IDialogState<SetActiveAssignmentDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly IAssignmentsClient _assignmentsClient;

        public SetActiveAssignmentInitialState(IAssignmentsClient assignmentsClient, ITelegramBotClient bot)
        {
            _assignmentsClient = assignmentsClient;
            _bot = bot;
        }

        public async Task Handle(SetActiveAssignmentDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var activeAssignments = (await _assignmentsClient.GetActiveAssignments(dialog.UserId) ?? Array.Empty<AssignmentDto>()).ToArray();

            if (activeAssignments.Length == 0)
            {
                await _bot.SendTextMessageAsync(dialog.UserId, "You have no current assignments at this time.");
                dialog.State = null;
                return;
            }

            var inlineButtons =
                activeAssignments.Select(x => new KeyboardButton($"Id: {x.Id}, Subject: {x.Subject}, Professor: {x.Assigner}"));
            var keyboard = new ReplyKeyboardMarkup(inlineButtons);

            await _bot.SendTextMessageAsync(dialog.UserId, "Select an assignment", replyMarkup:keyboard);

            dialog.State = new UserEntersAssignmentState(_bot, _assignmentsClient);
        }

        public Task Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as SetActiveAssignmentDialog, message);
        }
    }
}
