namespace Identity.Models.UniversityGroups
{
    public class UniversityGroup
    {
        public int Id { get; set; }

        public string? University { get; set; }

        public string? Faculty { get; set; }

        public int? Course { get; set; }

        public int? Group { get; set; }

        public Degree Degree { get; set; }
    }
}
