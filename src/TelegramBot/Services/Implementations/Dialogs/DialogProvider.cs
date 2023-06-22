using Microsoft.Extensions.Caching.Memory;
using Telegram.Bot;
using TelegramBot.Model.Exceptions;
using TelegramBot.Services.Implementations.Clients;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.GetStudentAssignmentsState;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.MeCommandState;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.SetActiveAssignmentState;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.SetGroupCommandState;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Interfaces.Clients;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Services.Implementations.Dialogs
{
    public class DialogProvider : IDialogProvider
    {
        private readonly ITelegramBotClient _bot;
        private readonly IMemoryCache _cache;
        private readonly StudentsClient _studentsClient;
        private readonly IUniversityGroupsService _groupsService;
        private readonly IAssignmentsClient _assignmentsClient;

        public DialogProvider(ITelegramBotClient bot, IMemoryCache cache, StudentsClient studentsClient, IUniversityGroupsService groupsService, IAssignmentsClient assignmentsClient)
        {
            _bot = bot;
            _cache = cache;
            _studentsClient = studentsClient;
            _groupsService = groupsService;
            _assignmentsClient = assignmentsClient;
        }

        public IDialog Create(string command, long userId) => command switch
        {
            "start" => new InitializeUserDialog(userId, 
                new InitialStartCommandState(_bot, _studentsClient), _cache),
            "me" => new UserInfoDialog(userId, new InitialMeCommandState(_studentsClient, _bot)),
            "setgroup" => new SetGroupUserDialog(userId, new InitialSetGroupCommandState(_groupsService, _bot), _cache),
            "getcurrentassignments" => new GetStudentAssignmentsDialog(userId, new InitialGetStudentAssignmentsCommandState(_bot, _assignmentsClient)),
            "setactiveassignment" => new SetActiveAssignmentDialog(userId, new SetActiveAssignmentInitialState(_assignmentsClient, _bot), _cache),
            "cancelactiveassignment" => throw new UnknownBotCommandException(command),
            "setassignement" => throw new UnknownBotCommandException(command),
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
