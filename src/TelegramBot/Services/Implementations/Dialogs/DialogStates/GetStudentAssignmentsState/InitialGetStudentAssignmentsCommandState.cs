using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.Clients;
using TelegramBot.Services.Interfaces.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.GetStudentAssignmentsState
{
    public class InitialGetStudentAssignmentsCommandState : IDialogState<GetStudentAssignmentsDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly IAssignmentsClient _assignmentsClient;

        public InitialGetStudentAssignmentsCommandState(ITelegramBotClient bot, IAssignmentsClient assignmentsClient)
        {
            _bot = bot;
            _assignmentsClient = assignmentsClient;
        }

        public async Task Handle(GetStudentAssignmentsDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var assignments = (await _assignmentsClient.GetAssignmentsByStudentId(dialog.UserId) ?? Array.Empty<AssignmentDto>()).ToArray();


            var assignmentsTotal = assignments.Length;

            if (assignmentsTotal > 0)
            {
                await _bot.SendTextMessageAsync(dialog.UserId, $"You have {assignmentsTotal} assignment(s):");
                foreach (var assignment in assignments)
                {
                    var now = DateTime.UtcNow;
                    var active = now switch
                    {
                        _ when assignment.StartDate > now => $"Active in {(assignment.StartDate - now).Minutes} minutes",
                        _ when assignment.EndDate >= now && now >= assignment.StartDate => "Active now",
                        _ when assignment.EndDate < now => "Overdue",
                        _ => throw new ArgumentOutOfRangeException()
                    };


                    await _bot.SendTextMessageAsync(dialog.UserId, $"Id: {assignment.Id}\nAssigner: {assignment.Assigner}\nSubject: {assignment.Subject}\nStart date (UTC): {assignment.StartDate}\nEnd date (UTC): {assignment.EndDate}\n{active}");
                }
            }
            else
            {
                await _bot.SendTextMessageAsync(dialog.UserId, "You have no assignments.");
            }


            dialog.State = null;
        }

        public Task Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as GetStudentAssignmentsDialog, message);
        }
    }
}
