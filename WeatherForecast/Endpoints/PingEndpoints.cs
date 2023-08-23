using System.Diagnostics.CodeAnalysis;

namespace WeatherForecast.Endpoints
{
	[ExcludeFromCodeCoverage]
	public static class PingEndpoints
	{
		public static void MapEndpoints(WebApplication app)
		{
			app.MapGet("/ping", Ping);
		}

		internal static IResult Ping()
		{
			return Results.Text("pong");
		}
	}
}
