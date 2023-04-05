using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class UserInfoDialog : IDialog
    {
        public UserInfoDialog(long userId, IDialogState state)
        {
            UserId = userId;
            State = state;
        }
        public IDialogState? State { get; set; }
        public long UserId { get; }
    }
}
