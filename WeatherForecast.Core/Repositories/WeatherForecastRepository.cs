using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WeatherForecast.Core.Extensions;
using WeatherForecast.Core.Models.Config;
using WeatherForecast.Core.Models.Infrastructure;
using WeatherForecast.Core.Models.Responses.OpenWeatherMap;
using WeatherForecast.Core.Services;

namespace WeatherForecast.Core.Repositories
{
	public interface IWeatherForecastRepository
	{
		Task<ServiceResponse<CurrentWeatherDataResponse>> GetWeatherByCity(string city);
	}

	public class WeatherForecastRepository : IWeatherForecastRepository
	{
		private readonly AppConfig _config;
		private readonly ILogger<WeatherForecastRepository> _logger;
		private readonly IHttpRequestService _httpRequestService;

		public WeatherForecastRepository(IOptions<AppConfig> config, ILogger<WeatherForecastRepository> log, IHttpRequestService httpRequestService)
		{
			_config = config.Value;
			_logger = log;
			_httpRequestService = httpRequestService;
		}

		public async Task<ServiceResponse<CurrentWeatherDataResponse>> GetWeatherByCity(string city)
		{
			var response = new ServiceResponse<CurrentWeatherDataResponse>();

			// Get GeoCode for city
			var cityGeoCode = await _httpRequestService.Get<GeocodingResponse>(GetGeoCodeUri(city), null, null);
			if (!cityGeoCode.IsSuccessful)
			{
				_logger.LogError($"No geo code was found for {city}");
				return response.WithError(ServiceResponseErrorType.SystemError);
			}

			// Get weather for city
			var currentWeatherDataResponse = await _httpRequestService.Get<CurrentWeatherDataResponse>(GetWeatherDataUri(cityGeoCode), null, null);
			if (!currentWeatherDataResponse.IsSuccessful)
			{
				_logger.LogError($"No weather data was found for {city}");
				return response.WithError(ServiceResponseErrorType.SystemError);
			}

			return response.With(currentWeatherDataResponse);
		}

		private string GetGeoCodeUri(string city)
		{
			//http://api.openweathermap.org/geo/1.0/direct?q={city name},{state code},{country code}&limit={limit}&appid={API key}
			return $"{_config.OpenWeatherMapGeoCodeAPIBaseUrl}?q={city}&appid={_config.OpenWeatherMapAPIKey}";
		}

		private string GetWeatherDataUri(GeocodingResponse cityGeoCode)
		{
			//https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API key}
			return $"{_config.OpenWeatherMapCurrentWeatherAPIBaseUrl}?lat={cityGeoCode.lat}&lon={cityGeoCode.lon}&appid={_config.OpenWeatherMapAPIKey}";
		}
	}
}