using TelegramBot.Model.Clients;

namespace TelegramBot.Services.Interfaces.Clients;

public interface IUniversityGroupsService
{
    Task<IEnumerable<UniversityGroupDto?>> GetUniversityGroups(Func<UniversityGroupDto, bool>? filter);
    Task EnrollStudentToGroup(long studentId, int groupId);
}