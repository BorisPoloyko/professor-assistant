using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class UserEntersFirstNameState : IStartCommandState
    {
        public ITelegramBotClient Bot { get; }
        public StudentsClient StudentClient { get; }

        public UserEntersFirstNameState(ITelegramBotClient client, StudentsClient studentClient)
        {
            Bot = client;
            StudentClient = studentClient;
        }

        public async Task Handle(InitializeUserDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var firstName = message.Text;
            dialog.FirstName = firstName;

            await Bot.SendTextMessageAsync(dialog.UserId, "Please enter your last name");
            dialog.State = new UserEntersLastNameState(Bot, StudentClient);
        }

        Task IDialogState.Handle(Dialog dialog, Message message)
        {
            return Handle(dialog as InitializeUserDialog, message);
        }
    }
}
