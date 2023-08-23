using System.Diagnostics.CodeAnalysis;

namespace WeatherForecast.Core.Models.Config
{
	[ExcludeFromCodeCoverage]
	public class AppConfig
	{
		public const string SectionName = "WeatherForecast";
		public string OpenWeatherMapAPIKey { get; set; }
		public string OpenWeatherMapGeoCodeAPIBaseUrl { get; set; }
		public string OpenWeatherMapCurrentWeatherAPIBaseUrl { get; set; }
	}
}