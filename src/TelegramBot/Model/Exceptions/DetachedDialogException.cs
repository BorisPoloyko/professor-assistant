namespace TelegramBot.Model.Exceptions
{
    public class DetachedDialogException : Exception
    {
        public DetachedDialogException()
        { }
        public DetachedDialogException(string message) : base(message) { }
        public DetachedDialogException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
