using Identity.Models.Students;
using System.Threading.Tasks;

namespace Identity.DataAccess.Interfaces
{
    public interface IStudentsRepository
    {
        Task AddStudent(Student student);

        Task<Student> FindStudent(long id);

        Task AssignGroup(long studentId, int groupId);

        Task Update(Student student);
    }
}
