using Telegram.Bot.Types;

namespace TelegramBot.Services.Interfaces.Processing
{
    public interface IUpdateProcessor
    {
        Task ProcessUpdate(Update update, CancellationToken cancellationToken = default);
    }
}
