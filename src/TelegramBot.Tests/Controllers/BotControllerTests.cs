using Castle.Core.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Telegram.Bot;
using TelegramBot.Controllers;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Requests;

namespace TelegramBot.Tests.Controllers
{
    public class BotControllerTests
    {
        private readonly Mock<ITelegramBotClient> _botMock;
        private readonly Mock<IDialogFactory> _dialogFactoryMock;
        private readonly ILogger<BotController> _logger;
        

        public BotControllerTests()
        {
            _botMock = new Mock<ITelegramBotClient>();
            _dialogFactoryMock = new Mock<IDialogFactory>();
            _logger = new NullLogger<BotController>();
        }

        [Fact]
        public void HelloEndpoint_RetutnsOk()
        {
            var controller = new BotController(_botMock.Object, new NullLogger<BotController>(), _dialogFactoryMock.Object);

            var result = controller.Hello(new CancellationToken()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}