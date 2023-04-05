using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class InitialStartCommandState : IDialogState
    {
        public async Task Handle(Dialog dialog, Message message)
        {
            await dialog.Bot.SendTextMessageAsync(dialog.UserId, "Welcome to Assistant bot! Please enter your first name");
            dialog.State = new UserEntersFirstNameState();
        }
    }
}
