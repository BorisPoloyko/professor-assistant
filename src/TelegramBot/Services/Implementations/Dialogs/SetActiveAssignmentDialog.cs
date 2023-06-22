using Microsoft.Extensions.Caching.Memory;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class SetActiveAssignmentDialog : IDialog
    {
        private IDialogState? _state;
        public SetActiveAssignmentDialog(long userId, IDialogState state, IMemoryCache cache)
        {
            Cache = cache;
            State = state;
            UserId = userId;
        }

        public IMemoryCache Cache { get; set; }

        public IDialogState? State
        {
            get => _state;
            set
            {
                _state = value;
                Cache.Set($"{UserId}_dialog",
                    this);
            }
        }
        
        public long UserId { get; }
    }
}
