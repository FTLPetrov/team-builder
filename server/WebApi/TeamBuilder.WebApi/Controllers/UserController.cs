using Microsoft.AspNetCore.Mvc;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Data.Common.Security;
using System.ComponentModel.DataAnnotations;
using TeamBuilder.Data.Common;

namespace TeamBuilder.WebApi.Controllers
{
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService, IValidationService validationService) 
            : base(validationService)
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
                return NotFoundResponse();
            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<UserResponse>> Create([FromBody] UserCreateRequest dto)
        {
            return await ValidateAndExecuteAsync(dto, async (validatedDto) =>
            {
                try
                {
                    var user = await _userService.CreateAsync(
                        SanitizeInput(validatedDto.FirstName),
                        SanitizeInput(validatedDto.LastName),
                        validatedDto.Email,
                        validatedDto.UserName,
                        validatedDto.Password);
                    
                    return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
                }
                catch (Exception ex)
                {
                    return BadRequestResponse(ex.Message);
                }
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserResponse>> Update(Guid id, [FromBody] UserUpdateRequest dto)
        {
            return await ValidateAndExecuteAsync(dto, async (validatedDto) =>
            {
                try
                {
                    var currentUser = await _userService.GetByIdAsync(id);
                    if (currentUser == null)
                        return NotFoundResponse();

                    var firstName = !string.IsNullOrWhiteSpace(validatedDto.FirstName) 
                        ? SanitizeInput(validatedDto.FirstName) 
                        : currentUser.FirstName;
                    var lastName = !string.IsNullOrWhiteSpace(validatedDto.LastName) 
                        ? SanitizeInput(validatedDto.LastName) 
                        : currentUser.LastName;
                    var email = !string.IsNullOrWhiteSpace(validatedDto.Email) 
                        ? validatedDto.Email 
                        : currentUser.Email;
                    var userName = !string.IsNullOrWhiteSpace(validatedDto.UserName) 
                        ? validatedDto.UserName 
                        : currentUser.UserName;

                    var user = await _userService.UpdateAsync(id, firstName, lastName, email, userName, validatedDto.Password);
                    if (user == null)
                        return NotFoundResponse();
                    
                    return Ok(user);
                }
                catch (Exception ex)
                {
                    return BadRequestResponse(ex.Message);
                }
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _userService.DeleteAsync(id);
            if (!deleted)
                return NotFoundResponse();
            
                                return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserLoginResponse>> Login([FromBody] UserLoginRequest request)
        {
            return await ValidateAndExecuteAsync(request, async (validatedRequest) =>
            {
                var result = await _userService.LoginAsync(validatedRequest);
                if (result == null)
                {
                    return UnauthorizedResponse("Invalid email or password");
                }
                
                return Ok(result);
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return BadRequestResponse("No token provided");

            var success = await _userService.LogoutAsync(token);
            if (!success)
                return BadRequestResponse("Failed to logout");

            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {

            }
            return Ok(new { message = "Successfully logged out" });
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
                return UnauthorizedResponse("No token provided");

            var user = await _userService.GetCurrentUserAsync(token);
            if (user == null)
                return UnauthorizedResponse("Invalid or expired token");

            return Ok(user);
        }

        [HttpPut("profile-picture")]
        public async Task<ActionResult<UserResponse>> UpdateProfilePicture(IFormFile profilePicture)
        {
            if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not Guid userId)
            {
                return UnauthorizedResponse("No valid user found");
            }

            return await ValidateFileAndExecuteAsync(profilePicture, SecurityConfig.FileUpload.AllowedImageTypes, SecurityConfig.FileUpload.MaxFileSizeMB, async (validatedFile) =>
            {
                try
                {
                    var user = await _userService.UpdateProfilePictureAsync(userId, validatedFile);
                    if (user == null)
                        return NotFoundResponse();
                    
                    return Ok(user);
                }
                catch (Exception ex)
                {
                    return BadRequestResponse(ex.Message);
                }
            });
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            return await ValidateAndExecuteAsync(request, async (validatedRequest) =>
            {
                try
                {
                    var token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                    if (string.IsNullOrEmpty(token))
                        return UnauthorizedResponse("No token provided");

                    var currentUser = await _userService.GetCurrentUserAsync(token);
                    if (currentUser == null)
                        return UnauthorizedResponse("Invalid or expired token");

                    var success = await _userService.ChangePasswordAsync(currentUser.Id, validatedRequest.CurrentPassword, validatedRequest.NewPassword);
                    if (!success)
                        return BadRequestResponse("Invalid current password or password change failed");

                    return Ok(new { message = "Password changed successfully" });
                }
                catch (Exception ex)
                {
                    return BadRequestResponse(ex.Message);
                }
            });
        }

        [HttpGet("warnings")]
        public async Task<ActionResult<IEnumerable<WarningResponse>>> GetUserWarnings()
        {
            try
            {
                if (!HttpContext.Items.TryGetValue("UserId", out var userIdObj) || userIdObj is not Guid userId)
                {
                    return UnauthorizedResponse("No valid user found");
                }

                var warnings = await _userService.GetUserWarningsAsync(userId);
                return Ok(warnings);
            }
            catch (Exception ex)
            {
                return BadRequestResponse(ex.Message);
            }
        }
    }

    public class ChangePasswordRequest
    {
        [Required(ErrorMessage = "Current password is required")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        [StrongPassword(ErrorMessage = "New password does not meet security requirements")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
