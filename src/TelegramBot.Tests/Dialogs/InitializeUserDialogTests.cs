using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Services.Implementations.Dialogs;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Implementations.HttpClients;

namespace TelegramBot.Tests.Dialogs
{
    public class InitializeUserDialogTests
    {
        private readonly long _userId;
        private readonly Mock<ITelegramBotClient> _botClientMock;
        private readonly Mock<StudentsClient> _studentsClientMock;
        private readonly IMemoryCache _memoryCache;
        public InitializeUserDialogTests()
        {
            _userId = 1;
            _botClientMock = new Mock<ITelegramBotClient>();
            _studentsClientMock = new Mock<StudentsClient>(null);
            var options = new MemoryCacheOptions();
            _memoryCache = new MemoryCache(options);
        }

        [Fact]
        public async Task Handle_DialogInitialized_ChainOfStateExecuted()
        {
            var startState = new InitialStartCommandState(_botClientMock.Object, _studentsClientMock.Object);
            var dialog = new InitializeUserDialog(_userId, startState, _memoryCache);
            var message = new Message();

            await dialog.Handle(message);
            
            Assert.Equal(typeof(UserEntersFirstNameState), dialog.State.GetType());

            await dialog.Handle(message);

            Assert.Equal(typeof(UserEntersLastNameState), dialog.State.GetType());

            await dialog.Handle(message);

            Assert.Null(dialog.State);
        }

        [Fact]
        public async Task Handle_HasNoState_ExceptionIsThrown()
        {
            var startState = new UserEntersLastNameState(_botClientMock.Object, _studentsClientMock.Object);
            var dialog = new InitializeUserDialog(_userId, startState, _memoryCache);
            var message = new Message();

            await dialog.Handle(message);

            Assert.Null(dialog.State);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await dialog.Handle(message));
        }
    }
}
