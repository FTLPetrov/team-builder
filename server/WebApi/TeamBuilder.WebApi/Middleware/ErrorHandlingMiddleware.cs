using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.WebApi.Configuration;
using System.Net;
using System.Text.Json;

namespace TeamBuilder.WebApi.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var errorResponse = new
            {
                message = ApplicationConstants.ErrorMessages.UnexpectedError,
                timestamp = DateTime.UtcNow,
                requestId = context.TraceIdentifier
            };

            switch (exception)
            {
                case UnauthorizedAccessException:
                    response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    errorResponse = new { message = ApplicationConstants.ErrorMessages.UnauthorizedAccess, timestamp = DateTime.UtcNow, requestId = context.TraceIdentifier };
                    break;

                case ArgumentException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { message = exception.Message, timestamp = DateTime.UtcNow, requestId = context.TraceIdentifier };
                    break;

                case InvalidOperationException:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse = new { message = exception.Message, timestamp = DateTime.UtcNow, requestId = context.TraceIdentifier };
                    break;

                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    break;
            }



            var result = JsonSerializer.Serialize(errorResponse);
            await response.WriteAsync(result);
        }
    }
}
