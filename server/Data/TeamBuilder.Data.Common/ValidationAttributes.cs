using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace TeamBuilder.Data.Common
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not string password)
                return new ValidationResult("Password is required");

            if (password.Length < 8)
                return new ValidationResult("Password must be at least 8 characters long");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return new ValidationResult("Password must contain at least one uppercase letter");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return new ValidationResult("Password must contain at least one lowercase letter");

            if (!Regex.IsMatch(password, @"\d"))
                return new ValidationResult("Password must contain at least one number");

            if (!Regex.IsMatch(password, @"[^A-Za-z0-9]"))
                return new ValidationResult("Password must contain at least one special character");

            return ValidationResult.Success;
        }
    }

    public class SafeStringAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || value is not string input)
                return ValidationResult.Success;

            // Check for potential XSS patterns
            var dangerousPatterns = new[]
            {
                @"<script",
                @"javascript:",
                @"on\w+\s*=",
                @"<iframe",
                @"<object",
                @"<embed"
            };

            foreach (var pattern in dangerousPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                    return new ValidationResult($"Input contains potentially dangerous content: {pattern}");
            }

            return ValidationResult.Success;
        }
    }

    public class FileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxSizeInMB;

        public FileSizeAttribute(int maxSizeInMB)
        {
            _maxSizeInMB = maxSizeInMB;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is IFormFile file)
            {
                if (file.Length > _maxSizeInMB * 1024 * 1024)
                    return new ValidationResult($"File size must be less than {_maxSizeInMB}MB");
            }

            return ValidationResult.Success;
        }
    }

    public class AllowedFileTypesAttribute : ValidationAttribute
    {
        private readonly string[] _allowedTypes;

        public AllowedFileTypesAttribute(params string[] allowedTypes)
        {
            _allowedTypes = allowedTypes;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;

            if (value is IFormFile file)
            {
                if (!_allowedTypes.Contains(file.ContentType.ToLower()))
                    return new ValidationResult($"File type not allowed. Allowed types: {string.Join(", ", _allowedTypes)}");
            }

            return ValidationResult.Success;
        }
    }
}
