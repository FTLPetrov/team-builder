using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Common.Security;

namespace TeamBuilder.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IValidationService _validationService;

        protected BaseController(IValidationService validationService)
        {
            _validationService = validationService;
        }

        protected ActionResult ValidateAndExecute<T>(T model, Func<T, ActionResult> action)
        {
            try
            {
                var validationResult = _validationService.ValidateModel(model);
                if (validationResult != ValidationResult.Success)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var result = action(model);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequestResponse("An error occurred while processing your request");
            }
        }

        protected async Task<ActionResult> ValidateAndExecuteAsync<T>(T model, Func<T, Task<ActionResult>> action)
        {
            try
            {
                var validationResult = _validationService.ValidateModel(model);
                if (validationResult != ValidationResult.Success)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var result = await action(model);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequestResponse("An error occurred while processing your request");
            }
        }

        protected async Task<ActionResult> ValidateFileAndExecuteAsync(IFormFile file, string[] allowedTypes, int maxSizeMB, Func<IFormFile, Task<ActionResult>> action)
        {
            try
            {
                var validationResult = _validationService.ValidateFile(file, allowedTypes, maxSizeMB);
                if (validationResult != ValidationResult.Success)
                {
                    return BadRequest(new { message = validationResult.ErrorMessage });
                }

                var result = await action(file);
                return result;
            }
            catch (Exception ex)
            {
                return BadRequestResponse("An error occurred while processing the file");
            }
        }

        protected string SanitizeInput(string input, bool forDatabase = false)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var sanitized = _validationService.SanitizeInput(input, forDatabase);
            return sanitized;
        }

        protected bool ContainsDangerousContent(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var isDangerous = _validationService.ContainsDangerousContent(input);
            return isDangerous;
        }

        protected Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return null;

            return userId;
        }

        protected bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        protected ActionResult UnauthorizedResponse(string message = "Unauthorized")
        {
            return Unauthorized(new { message });
        }

        protected ActionResult NotFoundResponse(string message = "Resource not found")
        {
            return NotFound(new { message });
        }

        protected ActionResult BadRequestResponse(string message)
        {
            return BadRequest(new { message });
        }


    }
}
