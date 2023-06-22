using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    // ISP violation ?
    public class InitialStartCommandState : IDialogState<InitializeUserDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StudentsClient _studentClient;

        public InitialStartCommandState(ITelegramBotClient bot, StudentsClient studentClient)
        {
            _bot = bot;
            _studentClient = studentClient;
        }

        public async Task Handle(InitializeUserDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            await _bot.SendTextMessageAsync(dialog.UserId, "Welcome to Assistant bot! Please enter your first name");
            dialog.State = new UserEntersFirstNameState(_studentClient, _bot);
        }

        Task IDialogState.Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as InitializeUserDialog, message);
        }
    }
}
