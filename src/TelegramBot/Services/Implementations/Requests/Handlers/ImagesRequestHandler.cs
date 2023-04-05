using MediatR;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types;
using Telegram.Bot;
using System.IO.Compression;
using TelegramBot.Model.Configurations;
using Microsoft.Extensions.Options;
using TelegramBot.Model.Requests;

namespace TelegramBot.Services.Implementations.Requests.Handlers
{
    public class ImagesRequestHandler : IRequestHandler<GetFilesRequest>
    {
        private readonly ITelegramBotClient _bot;
        private readonly AccessConfiguration _settings;

        public ImagesRequestHandler(ITelegramBotClient bot, IOptions<AccessConfiguration> settings)
        {
            _bot = bot;
            _settings = settings.Value;
        }
        public async Task Handle(GetFilesRequest request, CancellationToken cancellationToken)
        {
            // cross-cutting concern to be moved in a decorator
            if (!_settings.AdminIds.Contains(request.ChatId))
            {
                throw new InvalidOperationException($"User {request.Username} has no access to invoke ${nameof(GetFilesRequest)}");
            }
            // move to IImagesProvider
            IEnumerable<string> files = Directory.GetFiles("/app/photos");

            if (!string.IsNullOrEmpty(request.Username))
            {
                files = files.Where(x => x.Contains($"{request.Username}"));
            }

            // create decorator for zip

            if (files.Any())
            {
                if (request.IsZip)
                {
                    var archiveName = $"/app/photos/{request.Username}.zip";
                    using var zip = ZipFile.Open(archiveName, ZipArchiveMode.Create);
                    foreach (var file in files)
                    {
                        zip.CreateEntryFromFile(file, file);
                    }
                    zip.Dispose();
                    var content = System.IO.File.OpenRead(archiveName);
                    await _bot.SendDocumentAsync(request.ChatId, document: new InputOnlineFile(content: content, archiveName), cancellationToken: cancellationToken);
                    System.IO.File.Delete(archiveName);
                }
                else
                {
                    foreach (var file in files)
                    {
                        await using var data = System.IO.File.OpenRead(file);
                        await _bot.SendDocumentAsync(request.ChatId, document: new InputOnlineFile(content: data, file), cancellationToken: cancellationToken); 
                    }
                }
            }

            
        }
    }
}
