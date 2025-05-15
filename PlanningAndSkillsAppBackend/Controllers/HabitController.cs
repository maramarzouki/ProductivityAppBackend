using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.HabitService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [Route("api/habit")]
    [ApiController]
    public class HabitController : ControllerBase
    {
        private readonly IHabitService _habitService;
        public HabitController(IHabitService habitService)
        {
            _habitService = habitService;
        }

        [HttpPost("createHabit")]
        public async Task<IActionResult> AddHabit([FromBody] HabitModel habit)
        {
            try
            {
                var result = await _habitService.AddHabit(habit);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getHabitsByChallengeID/{challengeId}")]
        public async Task<IActionResult> GetHabitsByChallengeID([FromRoute] int challengeId)
        {
            try
            {
                var result = await _habitService.GetHabitsByChallengeID(challengeId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("checkHabit/{habitId}")]
        public async Task<IActionResult> CheckHabit([FromRoute] int habitId, [FromBody] bool isChecked)
        {
            try
            {
                var result = await _habitService.CheckHabit(isChecked, habitId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("updateHabit")]
        public async Task<IActionResult> UpdateHabit([FromBody] HabitModel habit)
        {
            try
            {
                var result = await _habitService.UpdateHabit(habit);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteHabit/{habitId}")]
        public async Task<IActionResult> DeleteHabit(int habitId)
        {
            try
            {
                var result = await _habitService.DeleteHabit(habitId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
