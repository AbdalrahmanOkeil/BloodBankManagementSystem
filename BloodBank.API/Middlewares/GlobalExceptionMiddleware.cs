using BloodBank.Application.Common;
using System.Net;
using System.Text.Json;

namespace BloodBank.API.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occured");

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var response = new ApiResponse<string>(false, "Something went wrong"/*ex.Message*/, default);

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
