using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    // ISP violation ?
    public class InitialStartCommandState : IStartCommandState
    {
        public ITelegramBotClient Bot { get; set; }
        public StudentsClient StudentClient { get; }

        public InitialStartCommandState(ITelegramBotClient client, StudentsClient studentClient)
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

            await Bot.SendTextMessageAsync(dialog.UserId, "Welcome to Assistant bot! Please enter your first name");
            dialog.State = new UserEntersFirstNameState(Bot, StudentClient);
        }

        Task IDialogState.Handle(Dialog dialog, Message message)
        {
            return Handle(dialog as InitializeUserDialog, message);
        }
    }
}
