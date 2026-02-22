using AuctionHub_backend.Core.Interfaces;
using AuctionHub_backend.Data.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuctionHub_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            var result = await _userService.RegisterAsync(dto);
            if (!result)
            {
                return BadRequest("Email already exists or registration failed.");
            }

            return Ok(new { message = "Registration successful" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {

            var result = await _userService.LoginAsync(dto);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            return Ok(result);
        }

        [HttpPut("password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var user = await _userService.GetByIdAsync(userId.Value);
            if (user == null)
            {
                return NotFound("Your account has been deleted and cannot be updated.");
            }

            var success = await _userService.UpdatePasswordAsync(userId.Value, dto);
            if (!success) return BadRequest("Current password is incorrect.");

            return NoContent();
        }

        //Admin

        [HttpPut("admin/{id}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BanUser(int id)
        {
            var ok = await _userService.BanUserAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }



        // GET: /api/user/admin
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }


        private int? GetCurrentUserId()
        {
            var idClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            return (idClaim != null && int.TryParse(idClaim.Value, out int id)) ? id : null;
        }
    }
}
