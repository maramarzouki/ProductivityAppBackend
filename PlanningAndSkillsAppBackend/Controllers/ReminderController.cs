using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.ReminderService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [ApiController]
    [Route("api/reminder")]
    public class ReminderController : ControllerBase
    {
        private readonly IReminderService _reminderService;
        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        [HttpPost("createReminder")]
        public async Task<IActionResult> CreateReminder([FromBody] ReminderModel reminder)
        {
            try
            {
                Console.WriteLine($"Received reminder: {JsonSerializer.Serialize(reminder)}");
                var result = await _reminderService.CreateReminder(reminder);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUserReminders/{userID}")]
        public async Task<IActionResult> GetUserReminders([FromRoute] int userID)
        {
            try
            {
                var result = await _reminderService.GetRemindersByUserId(userID);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("toggleReminder/{reminderID}/{userID}")]
        public async Task<IActionResult> ToggleReminder([FromRoute] int reminderID, [FromRoute] int userID, [FromBody] bool isActive)
        {
            try
            {
                var result = await _reminderService.ToggleReminder(reminderID, userID, isActive);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteReminder/{reminderID}")]
        public async Task<IActionResult> DeleteReminder([FromRoute] int reminderID)
        {
            try
            {
                var result = await _reminderService.DeleteReminder(reminderID);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
