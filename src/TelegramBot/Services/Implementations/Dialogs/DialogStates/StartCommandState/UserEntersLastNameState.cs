using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class UserEntersLastNameState : IDialogState
    {
        public async Task Handle(Dialog dialog, Message message)
        {
            var lastName = message.Text;
            var userDialog = dialog as InitializeUserDialog;
            if (userDialog != null)
            {
                userDialog.LastName = lastName;
            }

            try
            {
                await dialog.FinishDialog();
            }
            catch (Exception ex)
            {
                await dialog.Bot.SendTextMessageAsync(dialog.UserId, "Unable to register user. Please repeat the process or contact administrator");
                throw;
            }
            await dialog.Bot.SendTextMessageAsync(dialog.UserId, $"Your are registered as {userDialog.FirstName} {userDialog.LastName}");

        }
    }
}
