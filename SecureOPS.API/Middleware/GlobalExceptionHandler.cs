using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SecureOPS.Applications;
using SecureOPS.Applications.Exceptions;
using SecureOPS.Domain;
using static System.Net.Mime.MediaTypeNames;


namespace SecureOPS.WebAPI.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;
	private readonly IHostEnvironment _env;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment env)
	{
		_logger = logger;
		_env = env;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		_logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

		// 2. Map the Exception to the correct Status Code
		var statusCode = exception switch
		{
			// Custom Exceptions
			NotFoundException => (int)HttpStatusCode.NotFound,
			AppValidationException => (int)HttpStatusCode.BadRequest,
			ForbiddenException => (int)HttpStatusCode.Forbidden,

			// Standard .NET Exceptions
			KeyNotFoundException => (int)HttpStatusCode.NotFound,
			UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
			ArgumentException => (int)HttpStatusCode.BadRequest,
			NotImplementedException => (int)HttpStatusCode.NotImplemented,

			// Database Exceptions
			DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,
			DbUpdateException => (int)HttpStatusCode.BadRequest, // Usually Foreign Key/Unique constraint issues

			// Default
			_ => (int)HttpStatusCode.InternalServerError
		};
		// 3. Create the response using your ErrorResponse model
		var response = new ErrorResponse(
			statusCode,
			exception.Message,
			_env.IsDevelopment() ? exception.StackTrace : null
		);

		httpContext.Response.StatusCode = statusCode;
		httpContext.Response.ContentType = "application/json";

		await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

		return true;
	}
}

// --- Custom Exceptions Defined Here ---

