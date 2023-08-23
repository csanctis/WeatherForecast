using System.Diagnostics.CodeAnalysis;
using WeatherForecast.Endpoints;

namespace WeatherForecast.Extensions
{
	[ExcludeFromCodeCoverage]
	public static class EndpointExtensions
	{
		public static void UseEndpointExtensions(this WebApplication app)
		{
			PingEndpoints.MapEndpoints(app);
			WeatherForecastEndpoints.MapEndpoints(app);
		}
	}
}