namespace TelegramBot.Services.Interfaces.Dialogs
{
    public interface IDialogFactory
    {
        IDialog Create(string command, long userId);
        IDialog Extract(long userId);
    }
}
