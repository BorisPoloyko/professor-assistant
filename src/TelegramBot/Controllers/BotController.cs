using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using TelegramBot.Services.Implementations.Requests;
using TelegramBot.Services.Interfaces.Requests;

namespace TelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient _bot;
        private readonly IMediator _mediator;
        private readonly ILogger<BotController> _logger;
        private readonly IRequestFactory _requestFactory;

        public BotController(ITelegramBotClient bot, IMediator mediator, ILogger<BotController> logger, IRequestFactory requestFactory)
        {
            _bot = bot;
            _mediator = mediator;
            _logger = logger;
            _requestFactory = requestFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Received update: {}");
                switch (update.Type)
                {
                    // refactor towards service
                    case Telegram.Bot.Types.Enums.UpdateType.Message when
                    !string.IsNullOrEmpty(update.Message?.Text) &&
                    update.Message?.From?.Id != null:
                        var (commandName, args) = ParseCommand(update.Message.Text, update.Message.From.Id);
                        var request = _requestFactory.Create(commandName, args);
                        await _mediator.Send(request);
                        break;
                    case Telegram.Bot.Types.Enums.UpdateType.Message when
                        update.Message.Photo != null:
                        {
                            var fileId = update.Message.Photo.Last().FileId;
                            var fileInfo = await _bot.GetFileAsync(fileId);
                            var filePath = fileInfo.FilePath;
                            var photoFormat = fileInfo.FilePath.Split(".")[1];
                            var destination = $"/app/photos/{fileInfo.FileUniqueId}_{update.Message.From.Id}";
                            await using Stream fileStream = System.IO.File.OpenWrite(destination);
                            await _bot.DownloadFileAsync(
                                filePath: filePath,
                                destination: fileStream,
                                cancellationToken: cancellationToken);
                            await _bot.SendTextMessageAsync(update.Message.From.Id, "Photo received");
                            break;
                        }
                    case Telegram.Bot.Types.Enums.UpdateType.Message when
                    update.Message.Document != null:
                        {
                            var fileId = update.Message.Document.FileId;
                            var fileInfo = await _bot.GetFileAsync(fileId);
                            var filePath = fileInfo.FilePath;
                            var photoFormat = fileInfo.FilePath.Split(".")[1];
                            var destination = $"/app/photos/{fileInfo.FileUniqueId}_{update.Message.From.Id}";
                            await using Stream fileStream = System.IO.File.OpenWrite(destination);
                            await _bot.DownloadFileAsync(
                                filePath: filePath,
                                destination: fileStream,
                                cancellationToken: cancellationToken);
                            await _bot.SendTextMessageAsync(update.Message.From.Id, "File received");
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException("Unable to parse update");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error");
                _logger.LogError("Unable to process user message: {0}", update);
                var chatId = update.Message?.From?.Id ?? update.EditedMessage?.From?.Id;
                await _bot.SendTextMessageAsync(chatId, "Not supported message");
            }

            return Ok();
        }

        /// <summary>
        /// Testing endpoint that indicates that the application is running
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Hello(CancellationToken cancellationToken)
        {
            return Ok("Hello");
        }
        // move to another service
        private (string commandName, IReadOnlyCollection<string> commandArgs) ParseCommand(string text, long fromId)
        {
            if (!text.StartsWith("/"))
            {
                throw new ArgumentOutOfRangeException("Only commands are supported");
            }

            var command = text.Split();
            var commandName = command.First()[1..];
            var arguments = command.Skip(1);
            var args = new List<string> { fromId.ToString() };
            args.AddRange(arguments);
            return (commandName, args);
        }
    }
}
