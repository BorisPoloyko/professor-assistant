using MediatR;
using TelegramBot.Model.Requests;
using TelegramBot.Services.Interfaces.Requests;

namespace TelegramBot.Services.Implementations.Requests
{
    public class RequestFactory : IRequestFactory
    {
        private readonly Dictionary<string, Type> _map;

        public RequestFactory() 
        {
            _map = new Dictionary<string, Type>
            {
                { "files", typeof(GetFilesRequest)}
            };
        }

        public IRequest Create(string command, IEnumerable<string> arguments)
        {
            if (!_map.TryGetValue(command, out var requestType))
            {
                throw new ArgumentOutOfRangeException(nameof(command),"Unknown command" );
            }

            var request = Activator.CreateInstance(requestType, arguments) as IRequest;

            if (request == null) 
            {
                throw new ArgumentOutOfRangeException($"Unable to create reqeust of type: {requestType}");
            }

            return request;
        }
    }
}
