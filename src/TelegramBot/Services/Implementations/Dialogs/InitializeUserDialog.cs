using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class InitializeUserDialog : Dialog
    {
        public InitializeUserDialog(
            long userId,
            IStartCommandState state,
            IMemoryCache memoryCache) : base(userId, state)
        {
            Cache = memoryCache;
            Cache.Remove($"{userId}_dialog");
        }

        public IMemoryCache Cache { get; set; }

        public override IDialogState? State
        {
            get => _state;
            set
            {
                _state = value;
                Cache.Set($"{UserId}_dialog", 
                    this,
                    new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
            }
        }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
