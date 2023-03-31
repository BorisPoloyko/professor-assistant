namespace Identity.Models.Exceptions
{
    public class EntryNotFoundException : Exception
    {
        public EntryNotFoundException()
        { }
        public EntryNotFoundException(string message) : base(message) { }
        public EntryNotFoundException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
