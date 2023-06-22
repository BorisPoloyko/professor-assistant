using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Interfaces.Dialogs
{
    public interface IDialog
    {
        IDialogState? State { get; set; }
        public long UserId { get; }

        async Task<bool> Handle(Message message)
        {
            if (State == null)
            {
                return false;
            }

            await State.Handle(this, message);
            return true;
        }
    }
}
