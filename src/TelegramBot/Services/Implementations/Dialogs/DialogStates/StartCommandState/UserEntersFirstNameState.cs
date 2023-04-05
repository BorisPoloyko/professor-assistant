using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class UserEntersFirstNameState : IDialogState
    {
        public async Task Handle(Dialog dialog, Message message)
        {
            var firstName = message.Text;
            if (dialog is InitializeUserDialog userDialog)
            {
                userDialog.FirstName = firstName;
            }
            await dialog.Bot.SendTextMessageAsync(dialog.UserId, "Please enter your last name");
            dialog.State = new UserEntersLastNameState();
        }
    }
}
