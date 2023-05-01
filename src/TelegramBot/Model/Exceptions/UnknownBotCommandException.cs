namespace TelegramBot.Model.Exceptions
{
    public class UnknownBotCommandException : Exception
    {
        private readonly string _command;

        public UnknownBotCommandException(string command)
        {
            _command = command;
        }
        public UnknownBotCommandException(string message, string command) : base(message)
        {
            _command = command;
        }
        public UnknownBotCommandException(string message, Exception innerException, string command)
            : base(message, innerException)
        {
            _command = command;
        }
    }
}
