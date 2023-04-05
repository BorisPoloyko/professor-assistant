using Telegram.Bot.Types;

namespace TelegramBot.Services.Interfaces.Dialogs.DialogStates
{
    public interface IDialogState
    {
        Task Handle(Dialog dialog, Message message);
    }
}
