namespace TelegramBot.Model.Clients
{
    public record StudentDto
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
