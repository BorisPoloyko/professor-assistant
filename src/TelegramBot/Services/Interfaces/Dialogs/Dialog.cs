using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Interfaces.Dialogs
{
    public abstract class Dialog
    {
        protected IDialogState _state;
        public virtual IDialogState State { get; set; }
        public long UserId { get; }
        public ITelegramBotClient Bot { get; }
        protected Dialog(long userId, ITelegramBotClient bot, IDialogState state)
        {
            UserId = userId;
            Bot = bot;
            _state = state;
        }

        public async Task Handle(Message message)
        {
            await State.Handle(this, message);
        }

        public abstract Task FinishDialog();
    }

}
