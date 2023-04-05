using MediatR;

namespace TelegramBot.Model.Requests
{
    public class GetFilesRequest : IRequest
    {
        public GetFilesRequest(IReadOnlyList<string> args)
        {
            if (args.Count != 3 && args.Count != 2)
            {
                throw new ArgumentException("Invalid amount of arguments", nameof(args));
            }

            ChatId = args[0];
            Username = args[1];

            if (args.Count == 3)
            {
                Username = args[1];
                if (args[2].Equals("zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    IsZip = true;
                }
            }
        }

        public string ChatId { get; }
        public string? Username { get; }

        public bool IsAdminCommand { get; } = true;
        public bool IsZip { get; }
    }
}
