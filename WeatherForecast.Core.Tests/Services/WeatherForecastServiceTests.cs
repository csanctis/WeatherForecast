using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text.Json;
using WeatherForecasts.Core.Tests.Infrastructure;
using WeatherForecast.Core.Models.Config;
using WeatherForecast.Core.Models.Infrastructure;
using WeatherForecast.Core.Models.Responses.OpenWeatherMap;
using WeatherForecast.Core.Repositories;
using WeatherForecast.Core.Services;

namespace WeatherForecasts.Core.Tests.Services
{
	public class WeatherForecastServiceTests : BaseTests
	{
		private readonly WeatherForecastService _weatherForecastService;
		private readonly IOptions<AppConfig> _appConfig = Substitute.For<IOptions<AppConfig>>();
		private readonly ILogger<WeatherForecastService> _logger = Substitute.For<ILogger<WeatherForecastService>>();
		private readonly IWeatherForecastRepository _IWeatherForecastRepository = Substitute.For<IWeatherForecastRepository>();

		public WeatherForecastServiceTests()
		{
			_weatherForecastService = new WeatherForecastService(_appConfig, _logger, _IWeatherForecastRepository);
		}

		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Check_GetWeather_Success()
		{
			// Prepare
			var context = GetValidContextModel();

			_IWeatherForecastRepository.GetWeatherByCity(Arg.Any<string>()).Returns(new ServiceResponse<CurrentWeatherDataResponse>(GetWeatherResponseObj()));

			var weatherForecast = _weatherForecastService.GetResults(context, "brisbane");

			Assert.IsNotNull(weatherForecast);
			Assert.IsTrue(weatherForecast.IsCompletedSuccessfully);
			Assert.IsTrue(weatherForecast.Result.IsSuccessful);
			Assert.That(weatherForecast.Result.weather.Count, Is.AtLeast(1));
		}

		private CurrentWeatherDataResponse GetWeatherResponseObj()
		{
			var weatherAPIResponse = LoadJson(JsonFiles.WeatherDataResponse);
			return JsonSerializer.Deserialize<CurrentWeatherDataResponse>(weatherAPIResponse, _jsonOptions) ?? new();
		}
	}
}