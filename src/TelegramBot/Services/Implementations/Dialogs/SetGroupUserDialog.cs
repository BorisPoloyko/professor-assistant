using Microsoft.Extensions.Caching.Memory;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class SetGroupUserDialog : IDialog
    {
        private IDialogState? _state;

        public SetGroupUserDialog(long userId, IDialogState state, IMemoryCache cache)
        {
            _state = state;
            UserId = userId;
            Cache = cache;
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

        public UniversityGroupDto? Group { get; set; }
        
        public long UserId { get; }
    }
}
