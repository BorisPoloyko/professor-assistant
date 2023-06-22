namespace TelegramBot.Services.Interfaces.Dialogs
{
    public interface IDialogProvider
    {
        IDialog Create(string command, long userId);
        IDialog Extract(long userId);
    }
}
