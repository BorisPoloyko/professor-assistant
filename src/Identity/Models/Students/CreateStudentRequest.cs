namespace Identity.Models.Students
{
    public class CreateStudentRequest
    {
        public long Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }
    }
}
