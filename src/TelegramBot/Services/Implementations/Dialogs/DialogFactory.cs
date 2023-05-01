using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using TelegramBot.Model.Exceptions;
using TelegramBot.Model.Requests;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.MeCommandState;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.SetGroupCommandState;
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
        private readonly IUniversityGroupsService _groupsService;

        public DialogFactory(ITelegramBotClient bot, IMemoryCache cache, StudentsClient studentsClient, IUniversityGroupsService groupsService)
        {
            _bot = bot;
            _cache = cache;
            _studentsClient = studentsClient;
            _groupsService = groupsService;
        }

        public IDialog Create(string command, long userId) => command switch
        {
            "start" => new InitializeUserDialog(userId, 
                new InitialStartCommandState(_bot, _studentsClient), _cache),
            "me" => new UserInfoDialog(userId, new InitialMeCommandState(_studentsClient, _bot)),
            "setgroup" => new SetGroupUserDialog(userId, new InitialSetGroupCommandState(_groupsService, _bot), _cache),
            _ => throw new UnknownBotCommandException(command)
        };

        public IDialog Extract(long userId)
        {
            if (_cache.TryGetValue($"{userId}_dialog", out IDialog dialog))
            {
                return dialog;
            }

            throw new DetachedDialogException("Cannot define command for a message");
        }
    }
}
