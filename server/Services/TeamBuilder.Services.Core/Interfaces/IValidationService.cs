using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IValidationService
    {
        ValidationResult ValidateModel(object model);
        IEnumerable<ValidationResult> ValidateModelWithDetails(object model);
        bool IsValid(object model);
        ValidationResult ValidateFile(IFormFile file, string[] allowedTypes, int maxSizeMB);
        string SanitizeInput(string input, bool forDatabase = false);
        bool ContainsDangerousContent(string input);
        ValidationResult ValidateBusinessRules(object model, string operation);
    }
}
