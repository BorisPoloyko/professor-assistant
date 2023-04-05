using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class UserEntersFirstNameState : IDialogState<InitializeUserDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StudentsClient _studentClient;

        public UserEntersFirstNameState(StudentsClient studentClient, ITelegramBotClient bot)
        {
            _studentClient = studentClient;
            _bot = bot;
        }

        public async Task Handle(InitializeUserDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var firstName = message.Text;
            dialog.FirstName = firstName;

            await _bot.SendTextMessageAsync(dialog.UserId, "Please enter your last name");
            dialog.State = new UserEntersLastNameState(_studentClient, _bot);
        }

        Task IDialogState.Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as InitializeUserDialog, message);
        }
    }
}
