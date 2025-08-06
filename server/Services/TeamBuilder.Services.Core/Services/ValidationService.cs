using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Common.Security;
using TeamBuilder.Data.Common;
using TeamBuilder.Services.Core.Contracts.User.Requests;

namespace TeamBuilder.Services.Core.Services
{
    public class ValidationService : IValidationService
    {
        public ValidationService()
        {
        }

        public ValidationResult ValidateModel(object model)
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(model);
                
                if (!Validator.TryValidateObject(model, context, validationResults, true))
                {
                    var errorMessage = string.Join("; ", validationResults.Select(v => v.ErrorMessage));
                    return new ValidationResult(errorMessage);
                }
                
                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult("An error occurred during validation");
            }
        }

        public IEnumerable<ValidationResult> ValidateModelWithDetails(object model)
        {
            try
            {
                var validationResults = new List<ValidationResult>();
                var context = new ValidationContext(model);
                
                Validator.TryValidateObject(model, context, validationResults, true);
                
                if (validationResults.Any())
                {
                    // Validation failed
                }
                
                return validationResults;
            }
            catch (Exception ex)
            {
                return new[] { new ValidationResult("An error occurred during validation") };
            }
        }

        public bool IsValid(object model)
        {
            return ValidateModel(model) == ValidationResult.Success;
        }

        public ValidationResult ValidateFile(IFormFile file, string[] allowedTypes, int maxSizeMB)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return new ValidationResult("No file provided");
                }

                if (file.Length > maxSizeMB * 1024 * 1024)
                {
                    return new ValidationResult($"File size must be less than {maxSizeMB}MB");
                }

                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    return new ValidationResult($"File type not allowed. Allowed types: {string.Join(", ", allowedTypes)}");
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult("An error occurred during file validation");
            }
        }

        public string SanitizeInput(string input, bool forDatabase = false)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                    return input;

                if (forDatabase)
                    return InputSanitizer.SanitizeForDatabase(input);

                return InputSanitizer.SanitizeText(input);
            }
            catch (Exception ex)
            {
                return string.Empty; // Return empty string on error to prevent injection
            }
        }

        public bool ContainsDangerousContent(string input)
        {
            try
            {
                return InputSanitizer.ContainsDangerousContent(input);
            }
            catch (Exception ex)
            {
                return true; // Assume dangerous on error for safety
            }
        }

        public ValidationResult ValidateBusinessRules(object model, string operation)
        {
            try
            {
                var errors = new List<string>();

                // Add business rule validations here
                switch (model)
                {
                    case UserCreateRequest userCreate:
                        errors.AddRange(ValidateUserCreationRules(userCreate));
                        break;
                    case UserUpdateRequest userUpdate:
                        errors.AddRange(ValidateUserUpdateRules(userUpdate));
                        break;
                    // Add more business rule validations as needed
                }

                if (errors.Any())
                {
                    var errorMessage = string.Join("; ", errors);
                    return new ValidationResult(errorMessage);
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult("An error occurred during business rule validation");
            }
        }

        private IEnumerable<string> ValidateUserCreationRules(UserCreateRequest request)
        {
            var errors = new List<string>();

            // Check if email domain is allowed (example business rule)
            if (!string.IsNullOrEmpty(request.Email) && !IsAllowedEmailDomain(request.Email))
            {
                errors.Add("Email domain is not allowed");
            }

            // Check username availability (would need to be implemented with repository)
            // This is just an example - in practice you'd check against the database

            return errors;
        }

        private IEnumerable<string> ValidateUserUpdateRules(UserUpdateRequest request)
        {
            var errors = new List<string>();

            // Add update-specific business rules here
            if (!string.IsNullOrEmpty(request.Email) && !IsAllowedEmailDomain(request.Email))
            {
                errors.Add("Email domain is not allowed");
            }

            return errors;
        }

        private bool IsAllowedEmailDomain(string email)
        {
            // Example business rule - check if email domain is allowed
            var domain = email.Split('@').LastOrDefault()?.ToLower();
            var allowedDomains = new[] { "gmail.com", "yahoo.com", "outlook.com", "teambuilder.com" };
            return !string.IsNullOrEmpty(domain) && allowedDomains.Contains(domain);
        }
    }
}
