using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.SetGroupCommandState
{
    public class UserEntersGroupState : IDialogState<SetGroupUserDialog>
    {
        private readonly ITelegramBotClient _bot;
        private readonly IEnumerable<UniversityGroupDto?> _groups;
        private readonly IUniversityGroupsService _groupsService;
        public UserEntersGroupState(IUniversityGroupsService groupsService, ITelegramBotClient bot, IEnumerable<UniversityGroupDto?> groups)
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

            var groupText = message.Text;
            if (!int.TryParse(groupText, out var group) || !_groups.Select(x => x?.Group).Contains(group))
            {
                await _bot.SendTextMessageAsync(dialog.UserId, "Cannot recognize group. Try command again");
                dialog.Cache.Remove($"{dialog.UserId}_dialog");
                dialog.State = null;
                return;
            }

            var finalGroup = _groups.SingleOrDefault(x => x?.Group == group);
            if (finalGroup != null) await _groupsService.EnrollStudentToGroup(dialog.UserId, finalGroup.Id);
            await _bot.SendTextMessageAsync(dialog.UserId, $"You have been enrolled to group {finalGroup.Group} of course {finalGroup.Course}");
            dialog.Cache.Remove($"{dialog.UserId}_dialog");
            dialog.State = null;
        }

        Task IDialogState.Handle(IDialog dialog, Message message)
        {
            return Handle(dialog as SetGroupUserDialog, message);
        }
    }
}
