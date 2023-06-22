using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Interfaces.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.SetGroupCommandState
{
    public class InitialSetGroupCommandState : IDialogState<SetGroupUserDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly IUniversityGroupsService _groupsService;
        public InitialSetGroupCommandState(IUniversityGroupsService groupsService, ITelegramBotClient bot)
        {
            _groupsService = groupsService;
            _bot = bot;
        }
        public async Task Handle(SetGroupUserDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var groups = (await _groupsService.GetUniversityGroups(x => x is { University: "BSU", Faculty: "MMF", Degree: Degree.Bachelor })).ToArray();

            var courses = groups.Select(x => x?.Course).ToArray();
            var coursesString = string.Join(',', courses);

            await _bot.SendTextMessageAsync(dialog.UserId, $"Please select a course from below:\n{coursesString}");

            dialog.State = new UserEntersCourseState(_groupsService, _bot, groups);
        }

        public Task Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as SetGroupUserDialog, message);
        }
    }
}
