using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.SetGroupCommandState;
using TelegramBot.Services.Interfaces.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.SetActiveAssignmentState
{
    public class UserEntersAssignmentState: IDialogState<SetActiveAssignmentDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly IAssignmentsClient _assignmentsClient;
        private static readonly Regex _regex = new(@"(?<=Id: )\d+");
        public UserEntersAssignmentState(ITelegramBotClient bot, IAssignmentsClient assignmentsClient)
        {
            _bot = bot;
            _assignmentsClient = assignmentsClient;
        }
        public async Task Handle(SetActiveAssignmentDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }
            var assignmentText = message.Text;
            var idString = _regex.Match(assignmentText).Value;
            if (string.IsNullOrEmpty(idString) || !long.TryParse(idString, out var id))
            {
                await _bot.SendTextMessageAsync(dialog.UserId, "Unable to set current assignment", replyMarkup: new ReplyKeyboardRemove());
                return;
            }

            var assignment = await _assignmentsClient.GetAssignmentById(id);
            dialog.Cache.Set($"{dialog.UserId}_assignment", assignment, absoluteExpiration: assignment.EndDate);

            await _bot.SendTextMessageAsync(dialog.UserId, $"Assignment {assignment.Id} set. You can now send pictures or documents as solution", replyMarkup: new ReplyKeyboardRemove());

            dialog.State = null;
        }

        public Task Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as SetActiveAssignmentDialog, message);
        }
    }
}
