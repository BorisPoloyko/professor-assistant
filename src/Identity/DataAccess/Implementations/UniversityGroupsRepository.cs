using System.Linq.Expressions;
using Identity.DataAccess.Context;
using Identity.DataAccess.Interfaces;
using Identity.Models.UniversityGroups;
using Microsoft.EntityFrameworkCore;

namespace Identity.DataAccess.Implementations
{
    public class UniversityGroupsRepository : IUniversityGroupsRepository
    {
        private readonly IdentityDbContext _context;

        public UniversityGroupsRepository(IdentityDbContext context)
        {
            _context = context;
        }

        // refactor towards more general filter
        public async Task<UniversityGroup> FindUniversityGroup(FindUniversityGroupsQuery? filter)
        {
            var filteredGroups = BuildFilterQuery(filter);

            return await filteredGroups.AsNoTracking().SingleOrDefaultAsync();
        }

        public async Task CreateGroup(UniversityGroup group)
        {
            _context.UniversityGroups.Add(group);
            await _context.SaveChangesAsync();
        }

        private IQueryable<UniversityGroup> BuildFilterQuery(FindUniversityGroupsQuery? group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            IQueryable<UniversityGroup> filteredGroups = _context.UniversityGroups;

            if (group.University != null)
            {
                filteredGroups = filteredGroups.Where(x => x.University == group.University);
            }

            if (group.Faculty != null)
            {
                filteredGroups = filteredGroups.Where(x => x.Faculty == group.Faculty);
            }

            if (group.Course.HasValue)
            {
                filteredGroups = filteredGroups.Where(x => x.Course == group.Course);
            }

            if (group.Group.HasValue)
            {
                filteredGroups = filteredGroups.Where(x => x.Group == group.Group);
            }

            if (group.Degree.HasValue)
            {
                filteredGroups = filteredGroups.Where(x => x.Degree == group.Degree);
            }

            return filteredGroups;
        }

        
    }
}
