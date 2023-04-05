using Telegram.Bot.Types;

namespace TelegramBot.Services.Interfaces.Dialogs.DialogStates
{
    public interface IDialogState
    {
        Task Handle(IDialog dialog, Message message);
    }
    public interface IDialogState<in T> : IDialogState where T : IDialog
    {
        Task Handle(T dialog, Message message);
    }
}
