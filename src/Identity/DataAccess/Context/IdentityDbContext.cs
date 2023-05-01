using Identity.Models.Students;
using Identity.Models.UniversityGroups;
using Microsoft.EntityFrameworkCore;

namespace Identity.DataAccess.Context
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<UniversityGroup> UniversityGroups { get; set; }

        public IdentityDbContext() 
        {}
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) 
        {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StudentsConfig());
            base.OnModelCreating(modelBuilder);
        }
    }
}
