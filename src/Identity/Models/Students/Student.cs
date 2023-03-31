using Identity.Models.UniversityGroups;

namespace Identity.Models.Students
{
    public class Student
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public UniversityGroup? Group { get; set; }
    }
}
