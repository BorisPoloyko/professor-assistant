using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using TelegramBot.Model.Exceptions;
using TelegramBot.Model.Requests;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Implementations.HttpClients;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class DialogFactory : IDialogFactory
    {
        private readonly ITelegramBotClient _bot;
        private readonly IMemoryCache _cache;
        private readonly StudentsClient _studentsClient;

        public DialogFactory(ITelegramBotClient bot, IMemoryCache cache, StudentsClient studentsClient)
        {
            _bot = bot;
            _cache = cache;
            _studentsClient = studentsClient;
        }

        public Dialog Create(string command, long userId) => command switch
        {
            "start" => new InitializeUserDialog(userId, _bot, new InitialStartCommandState(), _cache, _studentsClient),
            _ => throw new UnknownBotCommandException(command)
        };

        public Dialog Extract(long userId)
        {
            if (_cache.TryGetValue($"{userId}_dialog", out Dialog dialog))
            {
                return dialog;
            }

            throw new DetachedDialogException("Cannot define command for a message");
        }
    }
}
