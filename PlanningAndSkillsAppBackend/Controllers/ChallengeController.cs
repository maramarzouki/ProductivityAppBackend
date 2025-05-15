using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Model;
using Service.ChallengeService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [Route("api/challenge")]
    [ApiController]
    public class ChallengeController : ControllerBase
    {
        private readonly IChallengeService _challengeService;
        public ChallengeController(IChallengeService challengeService)
        {
            _challengeService = challengeService;
        }

        [HttpPost("createChallenge")]
        public async Task<IActionResult> CreateChallenge([FromBody] ChallengeModel challenge)
        {
            try
            {
                var res = await _challengeService.CreateChallenge(challenge);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getChallengeDetails/{challengeId}")]
        public async Task<IActionResult> GetChallengeById([FromRoute] int challengeId)
        {
            try
            {
                var res = await _challengeService.GetChallengeById(challengeId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUserChallenges/{userId}")]
        public async Task<IActionResult> GetUserChallenges([FromRoute] int userId)
        {
            try
            {
                var res = await _challengeService.GetUserChallenges(userId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }


        [HttpPatch("startChallenge/{challengeId}")]
        public async Task<IActionResult> StartChallenge([FromRoute] int challengeId)
        {
            try
            {
                var res = await _challengeService.StartChallenge(challengeId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPatch("restartChallenge/{challengeId}")]
        public async Task<IActionResult> RestartChallenge([FromRoute] int challengeId, [FromBody] DateTime startDate)
        {
            try
            {
                var res = await _challengeService.RestartChallenge(challengeId, startDate);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPatch("cancelChallenge/{challengeId}")]
        public async Task<IActionResult> CancelChallenge([FromRoute] int challengeId)
        {
            try
            {
                var res = await _challengeService.CancelChallenge(challengeId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("checkStatus/{challengeId}")]
        public async Task<IActionResult> CheckStatus([FromRoute] int challengeId)
        {
            try
            {
                var res = await _challengeService.CheckChallengeStatus(challengeId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("confirmToday/{challengeId}")]
        public async Task<IActionResult> ConfirmToday([FromRoute] int challengeId)
        {
            try
            {
                var res = await _challengeService.ConfirmToday(challengeId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("updateChallenge")]
        public async Task<IActionResult> UpdateChallenge([FromBody] ChallengeModel challenge)
        {
            try
            {
                var res = await _challengeService.UpdateChallenge(challenge);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpDelete("deleteChallenge/{challengeId}")]
        public async Task<IActionResult> DeleteChallenge([FromRoute] int challengeId)
        {
            try
            {
                var res = await _challengeService.DeleteChallenge(challengeId);
                return Ok(new { result = res });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}
