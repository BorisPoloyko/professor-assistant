using Identity.DataAccess.Interfaces;
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
        public async Task<ActionResult<UniversityGroup>> GetGroups([FromQuery] FindUniversityGroupsQuery query)
        {
            try
            {
                var groups = await _repository.FindUniversityGroups(query);
                return Ok(groups);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("{groupId:long}")]
        public async Task<ActionResult<UniversityGroup>> DeleteGroup([FromRoute] long groupId)
        {
            try
            {
                await _repository.DeleteGroup(groupId);
                return Ok();
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
