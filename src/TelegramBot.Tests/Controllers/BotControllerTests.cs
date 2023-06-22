using Azure.Storage.Blobs;
using Castle.Core.Logging;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Telegram.Bot;
using TelegramBot.Controllers;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Tests.Controllers
{
    public class BotControllerTests
    {
        private readonly Mock<ITelegramBotClient> _botMock;
        private readonly Mock<IDialogProvider> _dialogFactoryMock;
        private readonly ILogger<BotController> _logger;
        private readonly Mock<BlobServiceClient> _blobClient;


        public BotControllerTests(Mock<BlobServiceClient> blobClient)
        {
            _blobClient = blobClient;
            _botMock = new Mock<ITelegramBotClient>();
            _dialogFactoryMock = new Mock<IDialogProvider>();
            _logger = new NullLogger<BotController>();
        }
    }
}