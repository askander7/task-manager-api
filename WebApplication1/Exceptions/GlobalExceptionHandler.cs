using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		_logger.LogError(
			exception,
			"Unhandled exception occurred.");

		var response = new ProblemDetails
		{
			Status = StatusCodes.Status500InternalServerError,
			Title = "An unexpected error occurred.",
			Detail = "Something went wrong while processing your request."
		};

		httpContext.Response.StatusCode = 500;

		await httpContext.Response.WriteAsJsonAsync(
			response,
			cancellationToken);

		return true;
	}
}