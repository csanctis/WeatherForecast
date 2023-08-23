using System.Diagnostics.CodeAnalysis;

namespace WeatherForecast.Middlewares
{
	[ExcludeFromCodeCoverage]
	public class ContextMiddleware
	{
		private readonly RequestDelegate _next;

		public ContextMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext httpContext, ILogger<ContextMiddleware> logger)
		{
			logger.LogTrace("Context model being populated by middleware");
			await _next(httpContext);
		}
	}
}