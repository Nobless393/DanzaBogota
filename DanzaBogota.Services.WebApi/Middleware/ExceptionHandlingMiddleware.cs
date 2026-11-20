using DanzaBogota.Transversal.Common.Responses;
using Serilog;
using System.Net;
using System.Text.Json;

namespace DanzaBogota.Services.WebApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;

        public ExceptionHandlingMiddleware(RequestDelegate next, IWebHostEnvironment environment)
        {
            _next = next;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception processing request {Method} {Path}",
                    context.Request.Method, context.Request.Path);

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var title = "Ocurrió un error inesperado.";

                var envelope = new ApiResponse<object?>
                {
                    Success = false,
                    Message = title,
                    Data = _environment.IsDevelopment() ? ex.Message : null
                };

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(envelope, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }));
            }
        }
    }
}
