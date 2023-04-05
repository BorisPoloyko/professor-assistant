using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class InitializeUserDialog : Dialog
    {
        private readonly IMemoryCache _memoryCache;
        private readonly StudentsClient _client;

        public InitializeUserDialog(
            long userId, 
            ITelegramBotClient bot,
            IDialogState state,
            IMemoryCache memoryCache,
            StudentsClient client) : base(userId, bot, state)
        {
            _memoryCache = memoryCache;
            _client = client;
            _memoryCache.Remove($"{userId}_dialog");
        }

        public override IDialogState State
        {
            get => _state;
            set
            {
                _state = value;
                _memoryCache.Set($"{UserId}_dialog", 
                    this,
                    new MemoryCacheEntryOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(5)
                    });
            }
        }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
        public override async Task FinishDialog()
        {
            await _client.CreateOrUpdateStudent(new StudentDto
            {
                Id = UserId, 
                FirstName = FirstName,
                LastName = LastName
            });
            
            _memoryCache.Remove($"{UserId}_dialog");
        }
    }
}
