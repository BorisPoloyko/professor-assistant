using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class GetStudentAssignmentsDialog : IDialog
    {
        public GetStudentAssignmentsDialog(long userId, IDialogState state)
        {
            UserId = userId;
            State = state;
        }
        public IDialogState? State { get; set; }
        public long UserId { get; }
    }
}
