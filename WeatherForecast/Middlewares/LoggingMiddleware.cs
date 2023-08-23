using Microsoft.Extensions.Options;
using System.Diagnostics.CodeAnalysis;
using WeatherForecast.Core.Models.Config;

namespace WeatherForecast.Middlewares
{
	[ExcludeFromCodeCoverage]
	public class LoggingMiddleware
	{
		private readonly RequestDelegate _next;

		public LoggingMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task Invoke(HttpContext context, IOptions<AppConfig> jsonOptions, ILogger<LoggingMiddleware> logger)
		{
			logger.LogInformation("Request is logged");
			await _next(context);
		}
	}
}