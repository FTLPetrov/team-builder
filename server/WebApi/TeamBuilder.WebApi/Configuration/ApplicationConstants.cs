namespace TeamBuilder.WebApi.Configuration
{
    public static class ApplicationConstants
    {
        public static class Cors
        {
            public const string PolicyName = "AllowFrontend";
        }

        public static class SignalR
        {
            public const string ChatHubPath = "/chatHub";
        }

        public static class Swagger
        {
            public const string Title = "TeamBuilder API";
            public const string Version = "v1";
            public const string BearerScheme = "Bearer";
            public const string AuthorizationHeader = "Authorization";
        }

        public static class ErrorMessages
        {
            public const string UnexpectedError = "An unexpected error occurred";
            public const string UnauthorizedAccess = "Unauthorized access";
        }

        public static class LogMessages
        {
            public const string AdminUserSeeded = "Admin user seeded successfully! Username: {Username}";
            public const string AdminUserExists = "Admin user already exists and privileges confirmed!";
            public const string FailedToCreateAdmin = "Failed to create admin user: {Errors}";
            public const string ErrorSeedingAdmin = "Error seeding admin user";
        }
    }
}
