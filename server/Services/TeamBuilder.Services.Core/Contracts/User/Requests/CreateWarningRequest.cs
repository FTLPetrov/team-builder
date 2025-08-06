using System.ComponentModel.DataAnnotations;

namespace TeamBuilder.Services.Core.Contracts.User.Requests
{
    public class CreateWarningRequest
    {
        [Required(ErrorMessage = "Warning message is required")]
        [StringLength(1000, ErrorMessage = "Warning message cannot exceed 1000 characters")]
        public string Message { get; set; } = string.Empty;
    }
} 