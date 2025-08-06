using Microsoft.Extensions.Configuration;

namespace TeamBuilder.Data.Common.Security
{
    public static class SecurityConfig
    {
        private static IConfiguration? _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static class PasswordPolicy
        {
            public static int MinLength => _configuration?.GetValue<int>("Security:PasswordPolicy:MinLength") ?? 8;
            public static bool RequireUppercase => _configuration?.GetValue<bool>("Security:PasswordPolicy:RequireUppercase") ?? true;
            public static bool RequireLowercase => _configuration?.GetValue<bool>("Security:PasswordPolicy:RequireLowercase") ?? true;
            public static bool RequireDigit => _configuration?.GetValue<bool>("Security:PasswordPolicy:RequireDigit") ?? true;
            public static bool RequireSpecialCharacter => _configuration?.GetValue<bool>("Security:PasswordPolicy:RequireSpecialCharacter") ?? true;
        }

        public static class FileUpload
        {
            public static int MaxFileSizeMB => _configuration?.GetValue<int>("Security:FileUpload:MaxFileSizeMB") ?? 5;
            public static string[] AllowedImageTypes => _configuration?.GetSection("Security:FileUpload:AllowedImageTypes").Get<string[]>() ?? 
                new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            public static string[] AllowedDocumentTypes => _configuration?.GetSection("Security:FileUpload:AllowedDocumentTypes").Get<string[]>() ?? 
                new[] { "application/pdf", "application/msword", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };
        }

        public static class JWT
        {
            public static int ExpirationMinutes => _configuration?.GetValue<int>("JwtSettings:ExpirationInMinutes") ?? 60;
            public static int RefreshTokenExpirationDays => 7;
        }



        public static class CORS
        {
            public static string[] AllowedOrigins => _configuration?.GetSection("Security:CORS:AllowedOrigins").Get<string[]>() ?? 
                new[] { "http://localhost:5173", "http://localhost:3000" };
            public static string[] AllowedHeaders => _configuration?.GetSection("Security:CORS:AllowedHeaders").Get<string[]>() ?? 
                new[] { "Authorization", "Content-Type", "X-Requested-With", "Accept", "Origin" };
            public static string[] AllowedMethods => _configuration?.GetSection("Security:CORS:AllowedMethods").Get<string[]>() ?? 
                new[] { "GET", "POST", "PUT", "DELETE", "OPTIONS" };
            public static string[] ExposedHeaders => _configuration?.GetSection("Security:CORS:ExposedHeaders").Get<string[]>() ?? 
                new string[] { };
        }

        public static class Admin
        {
            public static string DefaultEmail => _configuration?.GetValue<string>("Admin:DefaultEmail") ?? "admin@teambuilder.com";
            public static string DefaultUsername => _configuration?.GetValue<string>("Admin:DefaultUsername") ?? "admin";
            public static string DefaultFirstName => _configuration?.GetValue<string>("Admin:DefaultFirstName") ?? "Admin";
            public static string DefaultLastName => _configuration?.GetValue<string>("Admin:DefaultLastName") ?? "User";
            public static string DefaultPassword => _configuration?.GetValue<string>("Admin:DefaultPassword") ?? "Admin123!";
        }
    }
}
