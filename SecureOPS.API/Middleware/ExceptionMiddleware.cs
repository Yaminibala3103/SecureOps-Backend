using SecureOPS.Domain;
using System.Net;
using System.Text.Json;

namespace SecureOPS.WebAPI.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		// Constructor: Dependency Injection
		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				// MUST be called with (context) and awaited
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Security Ops Pipeline Error: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			var response = new ErrorResponse
			{
				StatusCode = context.Response.StatusCode,
				Message = "Internal Security Service Error.",
				Details = _env.IsDevelopment() ? exception.Message : "Contact System Administrator."
			};

			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
			var json = JsonSerializer.Serialize(response, options);

			await context.Response.WriteAsync(json);
		}
	}
}