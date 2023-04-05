using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Interfaces.Dialogs
{
    public abstract class Dialog
    {
        protected IDialogState? _state;
        public virtual IDialogState? State { get; set; }
        public long UserId { get; }
        protected Dialog(long userId, IDialogState state)
        {
            UserId = userId;
            _state = state;
        }

        public async Task Handle(Message message)
        {
            if (State == null)
            {
                throw new InvalidOperationException("Dialog has no state. That means that is has ended.");
            }

            await State.Handle(this, message);
        }
    }

}
