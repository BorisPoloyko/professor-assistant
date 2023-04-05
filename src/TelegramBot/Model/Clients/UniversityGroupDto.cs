namespace TelegramBot.Model.Clients
{
    public record UniversityGroupDto
    {

        public int Id { get; set; }

        public string? University { get; set; }

        public string? Faculty { get; set; }

        public int? Course { get; set; }

        public int? Group { get; set; }

        public Degree Degree { get; set; }

    }

    public enum Degree
    {
        Bachelor,
        Master
    }
}
