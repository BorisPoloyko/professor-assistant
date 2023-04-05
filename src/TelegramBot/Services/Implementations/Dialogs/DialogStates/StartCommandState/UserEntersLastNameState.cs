using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public class UserEntersLastNameState : IStartCommandState
    {
        public ITelegramBotClient Bot { get; }
        public StudentsClient StudentClient { get; }

        public UserEntersLastNameState(ITelegramBotClient client, StudentsClient studentClient)
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

            var lastName = message.Text;
            dialog.LastName = lastName;

            try
            {
                await StudentClient.CreateOrUpdateStudent(new StudentDto
                {
                    Id = dialog.UserId,
                    FirstName = dialog.FirstName,
                    LastName = dialog.LastName
                });
            }
            catch
            {
                await Bot.SendTextMessageAsync(dialog.UserId,
                    "Unable to register user. Please repeat the process or contact administrator");
     
            }
            finally
            {
                dialog.Cache.Remove($"{dialog.UserId}_dialog");
            }
            await Bot.SendTextMessageAsync(dialog.UserId, $"Your are registered as {dialog.FirstName} {dialog.LastName}");

            dialog.State = null;
        }

        public Task Handle(Dialog dialog, Message message)
        {
            return Handle(dialog as InitializeUserDialog, message);
        }
    }
}
