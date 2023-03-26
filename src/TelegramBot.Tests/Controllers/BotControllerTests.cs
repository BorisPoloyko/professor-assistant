using Castle.Core.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Telegram.Bot;
using TelegramBot.Controllers;
using TelegramBot.Services.Interfaces.Requests;

namespace TelegramBot.Tests.Controllers
{
    public class BotControllerTests
    {
        private readonly Mock<ITelegramBotClient> _botMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IRequestFactory> _requestFactoryMock;
        private readonly ILogger<BotController> _logger;
        

        public BotControllerTests()
        {
            _botMock = new Mock<ITelegramBotClient>();
            _mediatorMock = new Mock<IMediator>();
            _requestFactoryMock = new Mock<IRequestFactory>();
            _logger = new NullLogger<BotController>();
        }

        [Fact]
        public void HelloEndpoint_RetutnsOk()
        {
            var controller = new BotController(_botMock.Object, _mediatorMock.Object, _logger, _requestFactoryMock.Object);

            var result = controller.Hello(new CancellationToken()) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}