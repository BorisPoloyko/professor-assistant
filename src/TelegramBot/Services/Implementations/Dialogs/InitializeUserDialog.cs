﻿using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot.Types;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Dialogs.DialogStates;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class InitializeUserDialog : IDialog
    {
        private IDialogState? _state;
        public InitializeUserDialog(
            long userId,
            IDialogState state,
            IMemoryCache memoryCache)
        {
            UserId = userId;
            Cache = memoryCache; 
            State = state;
        }

        public IMemoryCache Cache { get; }

        public IDialogState? State
        {
            get => _state;
            set
            {
                _state = value;
                Cache.Set($"{UserId}_dialog", 
                    this);
            }
        }

        public long UserId { get; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
