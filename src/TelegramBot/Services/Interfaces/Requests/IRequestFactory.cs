using MediatR;

namespace TelegramBot.Services.Interfaces.Requests
{
    public interface IRequestFactory
    {
        IRequest Create(string command, IEnumerable<string> arguments);
    }
}
