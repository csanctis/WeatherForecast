using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherForecast.Core.Extensions
{
	[ExcludeFromCodeCoverage]
	public static class JsonSerializerOptionsExtensions
	{
		public static JsonSerializerOptions ConfigSerializerOptions(this JsonSerializerOptions serializerOptions)
		{
			serializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			serializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			serializerOptions.Converters.Add(new JsonStringEnumConverter());
			return serializerOptions;
		}
	}
}