using System.Text.Json;

namespace WebApplication1.Middlewares
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		public ExceptionHandlingMiddleware(RequestDelegate next)
		{
			_next = next;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception)
			{
				context.Response.StatusCode = 500;
				context.Response.ContentType = "application/json";

				var response = new
				{
					statusCode = 500,
					message = "An unexpected error occurred."
				};

				await context.Response.WriteAsync(
					JsonSerializer.Serialize(response));
			}
		}

	}
}
