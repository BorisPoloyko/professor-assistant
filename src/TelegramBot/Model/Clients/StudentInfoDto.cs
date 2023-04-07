namespace TelegramBot.Model.Clients
{
    public record StudentInfoDto : StudentDto
    {
        public UniversityGroupDto? UniversityGroup { get; set; }
    }
}
