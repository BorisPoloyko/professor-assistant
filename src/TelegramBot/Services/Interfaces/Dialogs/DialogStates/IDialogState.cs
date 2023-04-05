using Telegram.Bot.Types;

namespace TelegramBot.Services.Interfaces.Dialogs.DialogStates
{
    public interface IDialogState
    {
        Task Handle(Dialog dialog, Message message);
    }
    public interface IDialogState<in T> : IDialogState where T : Dialog
    {
        Task Handle(T dialog, Message message);
    }
}
