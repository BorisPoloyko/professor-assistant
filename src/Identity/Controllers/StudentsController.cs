using Identity.DataAccess.Interfaces;
using Identity.Models.Exceptions;
using Identity.Models.Students;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsRepository _studentsRepository;

        public StudentsController(IStudentsRepository studentsRepository)
        {
            _studentsRepository = studentsRepository;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> RegisterStudent(CreateStudentRequest request)
        {
            try
            {
                var student = new Student
                {
                    Id = request.Id,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                await _studentsRepository.AddStudent(student);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        [Route("{studentId:long}")]
        public async Task<ActionResult> UpdateStudent(Student student)
        {
            try
            {
                await _studentsRepository.Update(student);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPut]
        [Route("{studentId:long}/enroll/{groupId:int}")]
        public async Task<ActionResult> EnrollStudentToGroup(long studentId, int groupId)
        {
            try
            {
                await _studentsRepository.AssignGroup(studentId, groupId);
                return Ok();
            }
            catch (EntryNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
