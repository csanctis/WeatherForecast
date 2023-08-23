using Microsoft.Extensions.DependencyInjection;
using WeatherForecast.Core.Repositories;
using WeatherForecast.Core.Services;

namespace WeatherForecast.Core.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static void AddCoreServices(this IServiceCollection services)
		{
			// Services
			services.AddSingleton<IWeatherForecastService, WeatherForecastService>();
			services.AddSingleton<IHttpRequestService, HttpRequestService>();

			// Repositories
			services.AddSingleton<IWeatherForecastRepository, WeatherForecastRepository>();
		}
	}
}