using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;

namespace TeamBuilder.WebApi.Controllers
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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
        {
            return Ok(await _userService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponse>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest dto)
        {
            // Validate password confirmation
            if (dto.Password != dto.ConfirmPassword)
            {
                return BadRequest(new { message = "Password and confirm password do not match" });
            }

            // Validate password strength
            if (dto.Password.Length < 6)
            {
                return BadRequest(new { message = "Password must be at least 6 characters long" });
            }

            try
            {
                var user = await _userService.CreateAsync(dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.Password);
                return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UserUpdateRequest dto)
        {
            try
            {
                var user = await _userService.UpdateAsync(id, dto.FirstName, dto.LastName, dto.Email, dto.UserName, dto.Password);
                if (user == null)
                    return NotFound();
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
                return NotFound();
            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] UserLoginRequest request)
        {
            var result = await _userService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = "Invalid email or password" });
            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "No token provided" });

            var success = await _userService.LogoutAsync(token);
            if (!success)
                return BadRequest(new { message = "Failed to logout" });

            return Ok(new { message = "Successfully logged out" });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "No token provided" });

            var user = await _userService.GetCurrentUserAsync(token);
            if (user == null)
                return Unauthorized(new { message = "Invalid or expired token" });

            return Ok(user);
        }

        [HttpPut("{id}/profile-picture")]
        public async Task<ActionResult<UserResponse>> UpdateProfilePicture(Guid id, [FromBody] UpdateProfilePictureRequest request)
        {
            var user = await _userService.UpdateProfilePictureAsync(id, request.ProfilePictureUrl);
            if (user == null)
                return NotFound();
            return Ok(user);
        }
    }

    public class UpdateProfilePictureRequest
    {
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
