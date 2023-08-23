using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace WeatherForecast.Middlewares
{
	[ExcludeFromCodeCoverage]
	public static class ExceptionHandlerMiddleware
	{
		public static void HandleException(this IApplicationBuilder app, ILoggerFactory loggerFactory)
		{
			app.Run(context =>
			{
				var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
				if (exceptionHandlerPathFeature != null)
				{
					var logger = loggerFactory.CreateLogger(typeof(ExceptionHandlerMiddleware));
					var ex = exceptionHandlerPathFeature.Error;
					var path = exceptionHandlerPathFeature.Path;

					logger.LogCritical(ex, "Application error!", path);
					if (ex is BadHttpRequestException badHttpRequestException)
					{
						logger.LogError(ex, "BadHttpRequestException occured in the application");
					}
					else if (ex is AggregateException aggregateException)
					{
						foreach (var ae in aggregateException.Flatten().InnerExceptions)
						{
							logger.LogError(ae, "Global task error: {message}", ae.Message);
						}
					}
					else
					{
						logger.LogError(ex, "Unhandled exception occured in the application");
					}

					context.Response.StatusCode = 500;
				}

				return Task.CompletedTask;
			});
		}
	}
}