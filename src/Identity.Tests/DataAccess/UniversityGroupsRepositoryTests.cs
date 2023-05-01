using Identity.DataAccess.Context;
using Identity.DataAccess.Implementations;
using Identity.Models.UniversityGroups;
using Identity.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Identity.Tests.DataAccess
{
    public class UniversityGroupsRepositoryTests
    {
        private readonly DbContextOptions<IdentityDbContext> _contextOptions;
        public UniversityGroupsRepositoryTests()
        {
            _contextOptions = InMemoryDatabaseHelper.CreateOptions<IdentityDbContext>();
        }

        [Fact]
        public async Task CreateGroup_GroupIsNotNull_GroupIsInDatabase()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var repository = new UniversityGroupsRepository(context);
            var group = new UniversityGroup
            {
                University = "BSU",
                Faculty = "MMF",
                Course = 1,
                Group = 9,
                Degree = Degree.Bachelor
            };

            await repository.CreateGroup(group);
            await repository.SaveChanges();

            Assert.Equal(1, context.UniversityGroups.Count());
        }

        [Fact]
        public async Task FindUniversityGroup_QueryIsCorrect_ReturnsSingleGroup()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(new[]
            {
                new UniversityGroup
                {
                    University = "BSU",
                    Faculty = "MMF",
                    Course = 1,
                    Group = 9,
                    Degree = Degree.Bachelor
                },
                new UniversityGroup
                {
                    University = "BSU",
                    Faculty = "MMF",
                    Course = 1,
                    Group = 8,
                    Degree = Degree.Bachelor
                },
            });
            await context.SaveChangesAsync();
            var repository = new UniversityGroupsRepository(context);

            var filter = new FindUniversityGroupsQuery
            {
                University = "BSU",
                Course = 1,
                Faculty = "MMF",
                Group = 9
            };

            var group = await repository.FindUniversityGroups(filter);

            Assert.Equal(group.FirstOrDefault().Group, 9);
        }

        [Fact]
        public async Task FindUniversityGroup_QueryIsCorrectManyGroups_ThrowsException()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            await context.AddRangeAsync(new[]
            {
                new UniversityGroup
                {
                    University = "BSU",
                    Faculty = "MMF",
                    Course = 1,
                    Group = 9,
                    Degree = Degree.Bachelor
                },
                new UniversityGroup
                {
                    University = "BSU",
                    Faculty = "MMF",
                    Course = 1,
                    Group = 8,
                    Degree = Degree.Bachelor
                },
            });
            await context.SaveChangesAsync();
            var repository = new UniversityGroupsRepository(context);

            var filter = new FindUniversityGroupsQuery
            {
                University = "BSU"
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async ()=> await repository.FindUniversityGroups(filter));
        }
    }
}
