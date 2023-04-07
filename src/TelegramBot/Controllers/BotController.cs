using MediatR;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services.Implementations.Dialogs;
using TelegramBot.Services.Implementations.Dialogs.DialogStates.StartCommandState;
using TelegramBot.Services.Interfaces.Dialogs;

namespace TelegramBot.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient _bot;
        private readonly ILogger<BotController> _logger;
        private readonly IDialogFactory _dialogFactory;

        public BotController(ITelegramBotClient bot,
            ILogger<BotController> logger,
            IDialogFactory dialogFactory)
        {
            _bot = bot;
            _logger = logger;
            _dialogFactory = dialogFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody] Update update, CancellationToken cancellationToken)
        {
            ValidateUpdateRequest(update);

            var senderId = update.Message!.From!.Id;
            HttpContext.Items["userId"] = senderId;
            if (!string.IsNullOrEmpty(update.Message!.Text))
            {
                var text = update.Message.Text;
                if (text.StartsWith("/"))
                {
                    var commandName = ParseCommand(update.Message.Text);
                    var newDialog = _dialogFactory.Create(commandName, senderId);
                    await newDialog.Handle(update.Message);
                }
                else
                {
                    var dialog = _dialogFactory.Extract(senderId);
                    await dialog.Handle(update.Message);
                }

            }
            else if (update.Message!.Photo != null)
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
            }
            else if (update.Message.Document != null)
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
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(update),"Not supported data format.");
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

        private void ValidateUpdateRequest(Update update)
        {
            if (update.Type != UpdateType.Message || update.Message == null)
            {
                throw new ArgumentOutOfRangeException(nameof(update), "Only not empty messages are allowed.");
            }

            if (update.Message?.From?.Id == null)
            {
                throw new ArgumentOutOfRangeException(nameof(update), "Cannot determine the sender.");
            }
        }

        // move to another service
        private string ParseCommand(string text)
        {
            var command = text.Split();
            if (command.Length > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(command), "Arguments are not supported");
            }

            var commandName = text[1..];
            return commandName;
        }
    }
}
