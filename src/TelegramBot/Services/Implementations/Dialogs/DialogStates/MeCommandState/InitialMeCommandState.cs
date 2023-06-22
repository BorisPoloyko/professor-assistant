using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.MeCommandState
{
    public class InitialMeCommandState : IDialogState<UserInfoDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly StudentsClient _studentClient;

        public InitialMeCommandState(StudentsClient studentClient, ITelegramBotClient bot)
        {
            _studentClient = studentClient;
            _bot = bot;
        }

        public async Task Handle(UserInfoDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var student = await _studentClient.GetStudentInfo(dialog.UserId);

            if (student == null)
            {
                await _bot.SendTextMessageAsync(dialog.UserId, "Unable to fetch user data.");
            }
            else
            {
                await _bot.SendTextMessageAsync(dialog.UserId, $"Subject:{student.FirstName} {student.LastName}\nFaculty:{student.Group?.Faculty}, course: {student.Group?.Course ?? null}, group:{student.Group?.Group}");
            }
            
            dialog.State = null;
        }

        Task IDialogState.Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as UserInfoDialog, message);
        }
    }
}
