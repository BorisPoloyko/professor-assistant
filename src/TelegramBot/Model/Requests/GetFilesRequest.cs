using MediatR;
using Telegram.Bot.Types;

namespace TelegramBot.Model.Commands
{
    public class GetFilesRequest : IRequest
    {
        private GetFilesRequest(string chatId, string username)
        {
            if (string.IsNullOrEmpty(chatId))
            {
                throw new ArgumentNullException(nameof(chatId));
            }

            ChatId = chatId;
            Username = username;
        }

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

        public bool IsAdminComand { get; } = true;
        public bool IsZip { get; }
    }
}
