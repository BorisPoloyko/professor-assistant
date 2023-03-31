using Identity.Models.UniversityGroups;

namespace Identity.DataAccess.Interfaces
{
    public interface IUniversityGroupsRepository
    {
        Task<UniversityGroup> FindUniversityGroup(FindUniversityGroupsQuery filter);
        Task CreateGroup(UniversityGroup group);
    }
}
