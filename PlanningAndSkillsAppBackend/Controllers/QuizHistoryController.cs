using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.QuizHistoryService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [Route("api/quizHistory")]
    [ApiController]
    public class QuizHistoryController : ControllerBase
    {
        private readonly IQuizHistoryService _quizHistoryService;
        public QuizHistoryController(IQuizHistoryService quizHistoryService)
        {
            _quizHistoryService = quizHistoryService;
        }
        [HttpPost("addQuizHistory")]
        public async Task<IActionResult> CreateProject([FromBody] QuizHistoryModel quizHistoryModel)
        {
            try
            {
                var result = await _quizHistoryService.AddQuizHistory(quizHistoryModel);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getQuizHistory/{userID}/{reportNb}")]
        public async Task<IActionResult> GetQuizHistory([FromRoute] int userID, int reportNb)
        {
            try
            {
                var result = await _quizHistoryService.GetHistoryQuiz(userID, reportNb);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("compareReports")]
        public async Task<IActionResult> GetProjectByID([FromBody] string prompt)
        {
            try
            {
                var result = await _quizHistoryService.CompareReports(prompt);  
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getQuizHistoryByUserID/{userId}")]
        public async Task<IActionResult> GetQuizHistoryByUserID([FromRoute] int userId)
        {
            try
            {
                var result = await _quizHistoryService.GetQuizHistoryByUserID(userId);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteQuizHistory/{quizHistoryId}")]
        public async Task<IActionResult> DeleteQuizHistory([FromRoute] int quizHistoryId)
        {
            try
            {
                var result = await _quizHistoryService.DeleteQuizHistory(quizHistoryId);
                return Ok(new { result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
