using Identity.DataAccess.Implementations;
using Identity.DataAccess.Interfaces;
using Identity.Models.Students;
using Identity.Models.UniversityGroups;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class GroupsController : ControllerBase
    {
        private readonly IUniversityGroupsRepository _repository;

        public GroupsController(IUniversityGroupsRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<UniversityGroup>> GetGroup([FromQuery] FindUniversityGroupsQuery query)
        {
            try
            {
                var group = await _repository.FindUniversityGroup(query);
                return Ok(group);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> CreateGroup(CreateGroupRequest request)
        {
            try
            {
                var group = new UniversityGroup
                {
                    University = request.University,
                    Course = request.Course,
                    Faculty = request.Faculty,
                    Group = request.Group,
                    Degree = request.Degree
                };
                await _repository.CreateGroup(group);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
