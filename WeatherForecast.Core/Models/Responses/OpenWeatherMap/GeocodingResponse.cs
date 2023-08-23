using WeatherForecast.Core.Services;

namespace WeatherForecast.Core.Models.Responses.OpenWeatherMap
{
	/// <summary>
	/// http://api.openweathermap.org/geo/1.0/direct?q={city name},{state code},{country code}&limit={limit}&appid={API key}
	/// </summary>
	public class GeocodingResponse : IHttpResponse
	{
		public string name { get; set; }
		public double lat { get; set; }
		public double lon { get; set; }
		public string country { get; set; }
		public string state { get; set; }
		public bool IsSuccessful { get; set; }
		public string Message { get; set; }
	}
}