namespace TelegramBot.Services.Interfaces.Dialogs
{
    public interface IDialogFactory
    {
        Dialog Create(string command, long userId);
        Dialog Extract(long userId);
    }
}
