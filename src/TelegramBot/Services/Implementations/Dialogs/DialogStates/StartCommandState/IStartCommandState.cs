using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState
{
    public interface IStartCommandState : IDialogState<InitializeUserDialog>
    {
        protected ITelegramBotClient Bot { get; }

        protected StudentsClient StudentClient { get; }
    }
}
