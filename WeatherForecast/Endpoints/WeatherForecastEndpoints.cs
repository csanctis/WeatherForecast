using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Core.Models.Responses.OpenWeatherMap;
using WeatherForecast.Core.Services;

namespace WeatherForecast.Endpoints
{
	public class WeatherForecastEndpoints
	{
		public static void MapEndpoints(WebApplication app)
		{
			app.MapGet("forecast", GetForecast).CacheOutput("Expire10"); // Using 10min caching policy
		}

		internal static async Task<CurrentWeatherDataResponse> GetForecast(
			[FromServices] IContextService contextService,
			[FromServices] IWeatherForecastService weatherForecastService,
			[FromQueryAttribute] string city)
		{
			return await weatherForecastService.GetResults(contextService.GetContext(), city);
		}
	}
}