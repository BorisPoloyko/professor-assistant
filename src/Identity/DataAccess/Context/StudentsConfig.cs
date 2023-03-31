using Identity.Models.Students;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.DataAccess.Context
{
    public class StudentsConfig : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.Property(p => p.Id).HasColumnName("TelegramId");
        }
    }
}
