using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.SkillsProgressService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [Route("api/skillsProgress")]
    [ApiController]
    public class SkillsProgressController : ControllerBase
    {
        private readonly ISkillsProgressService _skillsProgressService;
        public SkillsProgressController(ISkillsProgressService skillsProgressService)
        {
            _skillsProgressService = skillsProgressService;
        }

        [HttpPost("addSkillsProgress")]
        public async Task<IActionResult> CreateProject([FromBody] SkillsProgressModel skillsProgressModel)
        {
            try
            {
                var result = await _skillsProgressService.AddSkillsProgress(skillsProgressModel);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getSkillsProgress/{userID}")]
        public async Task<IActionResult> GetHistoryQuiz([FromRoute] int userID)
        {
            try
            {
                var result = await _skillsProgressService.GetSkillsProgress(userID);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getSkillsProgressByUserID/{userId}")]
        public async Task<IActionResult> GetSkillsProgressByUserID([FromRoute] int userId)
        {
            try
            {
                var result = await _skillsProgressService.GetSkillsProgressByUserID(userId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("updateSkillsProgress")]
        public async Task<IActionResult> UpdateSkillsProgress([FromBody] SkillsProgressModel skillsProgressModel)
        {
            try
            {
                var result = await _skillsProgressService.UpdateSkillsProgress(skillsProgressModel);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteSkillsProgress/{skillsProgressId}")]
        public async Task<IActionResult> DeleteSkillsProgress([FromRoute] int skillsProgressId)
        {
            try
            {
                var result = await _skillsProgressService.DeleteSkillsProgress(skillsProgressId);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
