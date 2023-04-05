using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.HttpClients;
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

            await _bot.SendTextMessageAsync(dialog.UserId, $"Your profile: {student}");

            dialog.State = null;
        }

        Task IDialogState.Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as UserInfoDialog, message);
        }
    }
}
