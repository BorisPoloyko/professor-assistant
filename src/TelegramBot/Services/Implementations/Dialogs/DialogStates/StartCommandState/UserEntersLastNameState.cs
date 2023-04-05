using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class UserEntersLastNameState : IDialogState<InitializeUserDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StudentsClient _studentClient;

        public UserEntersLastNameState(StudentsClient studentClient, ITelegramBotClient bot)
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

            var lastName = message.Text;
            dialog.LastName = lastName;

            try
            {
                await _studentClient.CreateOrUpdateStudent(new StudentDto
                {
                    Id = dialog.UserId,
                    FirstName = dialog.FirstName,
                    LastName = dialog.LastName
                });
            }
            catch
            {
                await _bot.SendTextMessageAsync(dialog.UserId,
                    "Unable to register user. Please repeat the process or contact administrator");
     
            }
            finally
            {
                dialog.Cache.Remove($"{dialog.UserId}_dialog");
            }
            await _bot.SendTextMessageAsync(dialog.UserId, $"Your are registered as {dialog.FirstName} {dialog.LastName}");

            dialog.State = null;
        }

        public Task Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as InitializeUserDialog, message);
        }
    }
}
