using Microsoft.AspNetCore.Mvc;
using Model;
using Model.DTOs.UserDTOs;
using Service.UserService;

namespace PlanningAndSkillsAppBackend.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            try
            {
                var result = await _userService.RegisterUser(user);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var token = await _userService.LoginUser(loginDTO);
                return Ok(new { msg = token });
            }
            catch (Exception e)
            {
                return Unauthorized(new { error = e.Message });
            }
        }

        [HttpPost("sendCode")]
        public async Task<IActionResult> SendCode([FromBody] string email)
        {
            try
            {
                var result = await _userService.SendCode(email);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPost("verifyCode")]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeDTO verifyCodeDTO)
        {
            try
            {
                var result = await _userService.VerifyCode(verifyCodeDTO);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpPut("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            try
            {
                var result = await _userService.UpdatePassword(resetPasswordDTO);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }

        [HttpGet("getUserByEmail/{email}")]
        public async Task<IActionResult> GetUserByEmail([FromRoute] String email)
        {
            try
            {
                var result = await _userService.GetUserByEmail(email);
                return Ok(new { message = result });
            }
            catch (Exception e)
            {
                return BadRequest(new { error = e.Message });
            }
        }
    }
}

