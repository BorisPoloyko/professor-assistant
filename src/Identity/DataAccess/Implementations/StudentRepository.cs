using Identity.DataAccess.Context;
using Identity.DataAccess.Interfaces;
using Identity.Models.Exceptions;
using Identity.Models.Students;
using Microsoft.EntityFrameworkCore;

namespace Identity.DataAccess.Implementations
{
    public class StudentRepository : IStudentsRepository
    {
        private readonly IdentityDbContext _context;

        public StudentRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<Student> FindStudent(long id)
        {
            return await _context.Students.FindAsync(id);
        }

        public async Task Update(Student student)
        {
            var dbStudent = await _context.Students
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.Id == student.Id);

            if (dbStudent == null)
            {
                throw new EntryNotFoundException($"Can't find student with Id {student.Id}");
            }

            dbStudent.FirstName = student.FirstName;
            dbStudent.LastName = student.LastName;

            await _context.SaveChangesAsync();
        }

        public async Task AddStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
        }

        public async Task AssignGroup(long studentId, int groupId)
        {
            var student = await _context.Students
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.Id == studentId);

            var group = await _context.UniversityGroups.FindAsync(groupId);

            if (student == null)
            {
                throw new EntryNotFoundException($"Student with id {studentId} not found");
            }

            if (group == null)
            {
                throw new EntryNotFoundException($"Group with id {groupId} not found");
            }

            student.Group = group;
            await _context.SaveChangesAsync();
        }
    }
}
