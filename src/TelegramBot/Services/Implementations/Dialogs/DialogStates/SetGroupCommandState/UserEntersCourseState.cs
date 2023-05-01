using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.SetGroupCommandState
{
    public class UserEntersCourseState : IDialogState<SetGroupUserDialog>
    {

        private readonly ITelegramBotClient _bot;
        private readonly IEnumerable<UniversityGroupDto?> _groups;
        private readonly IUniversityGroupsService _groupsService;
        public UserEntersCourseState(IUniversityGroupsService groupsService, ITelegramBotClient bot, IEnumerable<UniversityGroupDto?> groups)
        {
            _groupsService = groupsService;
            _bot = bot;
            _groups = groups;
        }

        public async Task Handle(SetGroupUserDialog? dialog, Message message)
        {
            if (dialog == null)
            {
                throw new ArgumentNullException(nameof(dialog));
            }

            var courseText = message.Text;
            if (!int.TryParse(courseText, out var course) || !_groups.Select(x => x?.Course).Contains(course))
            {
                await _bot.SendTextMessageAsync(dialog.UserId, "Cannot recognize course. Try command again");
                dialog.Cache.Remove($"{dialog.UserId}_dialog");
                dialog.State = null;
                return;
            }

            var groups = _groups.Where(x => x?.Course == course).ToArray();
            var groupsInt = groups.Select(x => x?.Group).ToArray();

            var coursesString = string.Join(',', groupsInt);

            await _bot.SendTextMessageAsync(dialog.UserId, $"Please select a group from below:\n{coursesString}");

            dialog.State = new UserEntersGroupState(_groupsService, _bot, _groups);
        }

        Task IDialogState.Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as SetGroupUserDialog, message);
        }
    }
}
