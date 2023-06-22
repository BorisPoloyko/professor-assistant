using TelegramBot.Model.Clients;

namespace TelegramBot.Services.Interfaces.Clients
{
    public interface IAssignmentsClient
    {
        Task<IEnumerable<AssignmentDto>?> GetAssignmentsByStudentId(long studentId);
        Task<AssignmentDto?> GetAssignmentById(long id);
        Task<IEnumerable<AssignmentDto>?> GetActiveAssignments(long studentId);
    }
}
