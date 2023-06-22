namespace TelegramBot.Model.Clients
{
    public record AssignmentDto(int Id, string University, string Faculty, string Course, string Subject,
        string Assigner, DateTime StartDate, DateTime EndDate);
}
