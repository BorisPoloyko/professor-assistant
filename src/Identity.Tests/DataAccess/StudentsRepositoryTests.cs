using Identity.DataAccess.Context;
using Identity.DataAccess.Implementations;
using Identity.Models.Students;
using Identity.Models.UniversityGroups;
using Identity.Tests.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Identity.Tests.DataAccess
{
    public class StudentsRepositoryTests
    {
        private readonly DbContextOptions<IdentityDbContext> _contextOptions;
        public StudentsRepositoryTests()
        {
            _contextOptions = InMemoryDatabaseHelper.CreateOptions<IdentityDbContext>();
        }

        [Fact]
        public async Task AddStudent_StudentCorrect_StudentAdded()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var repository = new StudentRepository(context);
            var student = new Student
            {
                Id = 1,
                FirstName = "Jon",
                LastName = "Doe",
                Group = new UniversityGroup
                {
                    University = "BSU",
                    Faculty = "MMF",
                    Group = 1,
                    Course = 1,
                    Degree = Degree.Master
                }
            };

            await repository.AddStudent(student);

            Assert.Equal(1, context.Students.Count());
            Assert.Equal(1, context.UniversityGroups.Count());
        }

        [Fact]
        public async Task AddStudent_StudentExistsWithSameId_ExceptionThrown()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var repository = new StudentRepository(context);
            var student = new Student
            {
                Id = 1,
                FirstName = "Jon",
                LastName = "Doe"
            };

            await repository.AddStudent(student);
            await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.AddStudent(student));
        }

        [Fact]
        public async Task FindStudent_StudentExists_StudentFound()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Add(new Student
            {
                Id = 1,
                FirstName = "Jon",
                LastName = "Doe"
            });

            var repository = new StudentRepository(context);
            var student = repository.FindStudent(1);

            Assert.Equal(1, student.Id);
        }

        [Fact]
        public async Task UpdateStudent_CorrectInfo_StudentUpdatedAndGroupNotDeleted()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Add(new Student
            {
                Id = 1,
                FirstName = "Jon",
                LastName = "Doe",
                Group = new UniversityGroup
                {
                    University = "BSU"
                }
            });

            await context.SaveChangesAsync();

            var repository = new StudentRepository(context);
            var student = repository.Update(new Student
            {
                Id = 1,
                FirstName = "Alex",
                LastName = "Doe"
            });

            var dbStudent = await context.Students.FirstOrDefaultAsync();
            var dbGroup = await context.UniversityGroups.FirstOrDefaultAsync();

            Assert.NotNull(dbStudent);
            Assert.NotNull(dbGroup);
            Assert.Equal("Alex", dbStudent.FirstName);
            Assert.Equal("Doe", dbStudent.LastName);
            Assert.Equal("BSU", dbGroup.University);
        }

        [Fact]
        public async Task AssignGroup_CorrectInfo_GroupAssignedToStudent()
        {
            await using var context = new IdentityDbContext(_contextOptions);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            context.Add(new Student
            {
                Id = 1,
                FirstName = "Jon",
                LastName = "Doe"
            });

            context.Add(new UniversityGroup
            {
                Id = 10,
                University = "BSU"
            });

            await context.SaveChangesAsync();

            var repository = new StudentRepository(context);
            await repository.AssignGroup(1, 10);

            var dbStudent = await context.Students.Include(x => x.Group).FirstOrDefaultAsync();
            Assert.NotNull(dbStudent);
            Assert.Equal(dbStudent.Group.Id, 10);
        }
    }
}
