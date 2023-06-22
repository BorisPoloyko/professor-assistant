using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Model.Clients;
using TelegramBot.Services.Implementations.Clients;
using TelegramBot.Services.Interfaces.Dialogs;
using TelegramBot.Services.Interfaces.Processing;

namespace TelegramBot.Services.Implementations.Processing
{
    public class UpdateProcessor : IUpdateProcessor
    {
        private readonly ITelegramBotClient _bot;
        private readonly IDialogProvider _dialogProvider;
        private readonly BlobServiceClient _blobClient;
        private readonly StudentsClient _studentsClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<UpdateProcessor> _logger;

        private const string MetadataKey = "user";

        //Move to config
        private const string BlobContainerName = "students-assignments";
        public UpdateProcessor(ITelegramBotClient bot, IDialogProvider dialogProvider, BlobServiceClient blobClient, StudentsClient studentsClient, IMemoryCache cache, ILogger<UpdateProcessor> logger)
        {
            _bot = bot;
            _dialogProvider = dialogProvider;
            _blobClient = blobClient;
            _studentsClient = studentsClient;
            _cache = cache;
            _logger = logger;
        }
        public async Task ProcessUpdate(Update update, CancellationToken cancellationToken = default)
        {
            var senderId = update.Message!.From!.Id;

            if (!string.IsNullOrEmpty(update.Message!.Text))
            {
                await ProcessText(update, senderId);
            }
            else if (update.Message!.Photo != null)
            {
                await ProcessPhoto(update, cancellationToken);
            }
            else if (update.Message.Document != null)
            {
                await ProcessDocument(update, cancellationToken);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(update), "Not supported data format.");
            }
        }

        private async Task ProcessText(Update update, long senderId)
        {
            var text = update.Message.Text;
            if (text.StartsWith("/"))
            {
                var commandName = ParseCommand(update.Message.Text);
                var newDialog = _dialogProvider.Create(commandName, senderId);
                await newDialog.Handle(update.Message);
            }
            else
            {
                var dialog = _dialogProvider.Extract(senderId);
                await dialog.Handle(update.Message);
            }
        }

        private async Task ProcessDocument(Update update, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue($"{update.Message.From.Id}_assignment", out AssignmentDto assignment))
            {
                await _bot.SendTextMessageAsync(update.Message.From.Id, "Unable to find an assignment to attach a file to.", cancellationToken: cancellationToken);
                return;
            }
            var fileId = update.Message.Document.FileId;
            await SaveData(update, cancellationToken, fileId, assignment);
        }

        

        private async Task ProcessPhoto(Update update, CancellationToken cancellationToken)
        {
            if (!_cache.TryGetValue($"{update.Message.From.Id}_assignment", out AssignmentDto assignment))
            {
                await _bot.SendTextMessageAsync(update.Message.From.Id, "Unable to find an assignment to attach a file to.", cancellationToken: cancellationToken);
                return;
            }
            var fileId = update.Message.Photo.Last().FileId;
            await SaveData(update, cancellationToken, fileId, assignment);
        }

        private async Task SaveData(Update update, CancellationToken cancellationToken, string fileId, AssignmentDto assignment)
        {
            try
            {
                var fileInfo = await _bot.GetFileAsync(fileId, cancellationToken: cancellationToken);
                var filePath = fileInfo.FilePath;
                var photoFormat = fileInfo.FilePath.Split(".")[1];

                await using var destinationStream = new MemoryStream();
                await _bot.DownloadFileAsync(
                    filePath: filePath,
                    destination: destinationStream,
                    cancellationToken: cancellationToken);
                var containerClient = _blobClient.GetBlobContainerClient(BlobContainerName);

                destinationStream.Position = 0;

                var metadata = new Dictionary<string, string>();
                var user = await _studentsClient.GetStudentInfo(update.Message.From.Id);
                metadata[MetadataKey] = $"{user.FirstName}_{user.LastName}";
                var name = $"{user.FirstName}_{user.LastName}_{update.Message.From.Id}_{fileInfo.FileUniqueId}.{photoFormat}";
                var blobClient = containerClient.GetBlobClient(name);
                await blobClient.UploadAsync(destinationStream, cancellationToken);
                await blobClient.SetMetadataAsync(metadata, cancellationToken: cancellationToken);
                await _bot.SendTextMessageAsync(update.Message.From.Id, "File received", cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to save image of user {0}", update.Message!.From!.Id);
            }
        }

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
