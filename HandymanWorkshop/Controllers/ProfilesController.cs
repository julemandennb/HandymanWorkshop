
using HandymanWorkshop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.ApplicationUser;

namespace HandymanWorkshop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ProfilesController : ControllerBase
    {
        private readonly IApplicationUserService _userService;

        public ProfilesController(
            IApplicationUserService userService)
        {
            _userService = userService;
        }

        // GET: api/Profiles
        [HttpGet]
        public async Task<ActionResult<List<ApplicationUserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsyncDTO();
            return Ok(users);
        }

        // POST: api/Profiles
        [HttpPost]
        public async Task<ActionResult<ApplicationUserDto>> Create(
            [FromBody] ApplicationNewUserDto dto)
        {
            try
            {
                var user = await _userService.NewUser(dto);

                if (user == null)
                {
                    return BadRequest(
                        "Passwords do not match.");
                }

                return Ok(user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // POST: api/Profiles/{id}/resend-password
        [HttpPost("{id}/resend-password")]
        public async Task<IActionResult> ResendPassword(string id)
        {
            var success = await _userService.ResendNewPasswordByIdAsync(id);

            if (!success)
            {
                return NotFound("User not found.");
            }

            return Ok(new { message = "New password sent." });
        }

        // DELETE: api/Profiles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var success = await _userService.DeleteUserAsync(id);

            if (!success)
            {
                return NotFound("User not found.");
            }

            return NoContent();
        }
    }
}
