using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.ProjectService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [ApiController]
    [Route("api/project")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _projecService;
        public ProjectController(IProjectService projectService)
        {
            _projecService = projectService;
        }

        [HttpPost("createProject")]
        public async Task<IActionResult> CreateProject([FromBody] ProjectModel project)
        {
            try
            {
                var result = await _projecService.CreateProject(project);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUserProjects/{userID}")]
        public async Task<IActionResult> GetUserProject([FromRoute] int userID)
        {
            try
            {
                var result = await _projecService.GetUserProjects(userID);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getProjectById/{projectId}")]
        public async Task<IActionResult> GetProjectByID([FromRoute] int projectId)
        {
            try
            {
                var result = await _projecService.GetProjectById(projectId);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("completeProject/{projectId}")]
        public async Task<IActionResult> CompleteProject([FromRoute] int projectId, [FromBody] bool isCompleted)
        {
            try
            {
                var result = await _projecService.CompleteProject(isCompleted, projectId);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new {error=e.Message});
            }
        }

        [HttpPut("updateProject")]
        public async Task<IActionResult> UpdateProject([FromBody] ProjectModel project)
        {
            try
            {
                var result = await _projecService.UpdateProject(project);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteProject/{projectId}")]
        public async Task<IActionResult> DeleteProject([FromRoute] int projectId)
        {
            try
            {
                var result = await _projecService.DeleteProject(projectId);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
