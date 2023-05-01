using Identity.Models.UniversityGroups;

namespace Identity.DataAccess.Interfaces
{
    public interface IUniversityGroupsRepository
    {
        Task<IEnumerable<UniversityGroup>> FindUniversityGroups(FindUniversityGroupsQuery filter);
        Task CreateGroup(UniversityGroup group);
        Task DeleteGroup(long groupId);

        Task SaveChanges();
    }
}
