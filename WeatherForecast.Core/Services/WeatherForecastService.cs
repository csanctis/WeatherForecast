using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherForecast.Core.Models.Config;
using WeatherForecast.Core.Models.Infrastructure;
using WeatherForecast.Core.Models.Responses.OpenWeatherMap;
using WeatherForecast.Core.Repositories;

namespace WeatherForecast.Core.Services
{
	public interface IWeatherForecastService
	{
		Task<CurrentWeatherDataResponse> GetResults(ContextBase context, string city);
	}

	public class WeatherForecastService : IWeatherForecastService
	{
		private readonly ILogger<WeatherForecastService> _logger;
		private readonly IOptions<AppConfig> _appConfig;
		private readonly IWeatherForecastRepository _iWeatherForecastRepository;

		public WeatherForecastService(IOptions<AppConfig> appConfig, ILogger<WeatherForecastService> logger, IWeatherForecastRepository iWeatherForecastRepository)
		{
			_appConfig = appConfig;
			_logger = logger;
			_iWeatherForecastRepository = iWeatherForecastRepository;
		}

		public async Task<CurrentWeatherDataResponse> GetResults(ContextBase context, string city)
		{
			_logger.LogTrace("Getting WeatherForecast Results");

			return await _iWeatherForecastRepository.GetWeatherByCity(city);
		}
	}
}