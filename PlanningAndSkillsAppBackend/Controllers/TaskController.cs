using Microsoft.AspNetCore.Mvc;
using Model;
using Service.TaskService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [ApiController]
    [Route("api/task")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }
        [HttpPost("createTask")]
        public async Task<IActionResult> CreateTask(TaskModel task)
        {
            try
            {
                var result = await _taskService.CreateTask(task);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getTaskById/{taskID}")]
        public async Task<IActionResult> GetTaskById([FromRoute] int taskID)
        {
            try
            {
                var result = await _taskService.GetTaskById(taskID);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUserTasks/{userID}")]
        public async Task<IActionResult> GetUserTasks([FromRoute] int userID)
        {
            try
            {
                var result = await _taskService.GetUserTasks(userID);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUserTasks/{userID}/{selectedDate}")]
        public async Task<IActionResult> GetUserTasks([FromRoute] int userID, [FromRoute] DateTime selectedDate)
        {
            try
            {
                var result = await _taskService.GetUserTasksByDate(userID, selectedDate);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        [HttpGet("getTasksByProjectId/{projectId}")]
        public async Task<IActionResult> GetTasksByProjectId([FromRoute] int projectId)
        {
            try
            {
                var result = await _taskService.GetTasksByProjectId(projectId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("checkTask/{taskId}")]
        public async Task<IActionResult> CheckTask([FromRoute] int taskId, [FromBody] bool isChecked)
        {
            try
            {
                var result = await _taskService.CheckTask(isChecked, taskId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteTask/{taskID}")]
        public async Task<IActionResult> DeleteTask([FromRoute] int taskID)
        {
            try
            {
                var result = await _taskService.DeleteTask(taskID);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("updateTask")]
        public async Task<IActionResult> UpdateTask([FromBody] TaskModel task)
        {
            try
            {
                var result = await _taskService.UpdateTask(task);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
