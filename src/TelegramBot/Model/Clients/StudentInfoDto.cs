namespace TelegramBot.Model.Clients
{
    public record StudentInfoDto : StudentDto
    {
        public UniversityGroupDto? Group { get; set; }
    }
}
