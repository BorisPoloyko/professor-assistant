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

        public async Task<Student?> FindStudent(long id)
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
        }

        public async Task Delete(long studentId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
            {
                throw new EntryNotFoundException($"Student with Id ${studentId} not found");
            }

            _context.Students.Remove(student);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public Task AddStudent(Student student)
        {
            _context.Students.Add(student);
            return Task.CompletedTask; 
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
        }
    }
}
