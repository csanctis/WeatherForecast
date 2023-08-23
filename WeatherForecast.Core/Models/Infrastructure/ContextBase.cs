using System.Text.Json.Serialization;

namespace WeatherForecast.Core.Models.Infrastructure
{
	public abstract record ContextBase
	{
		public Dictionary<string, string> Header { get; init; }
		public DateTimeOffset RequestStart { get; init; }

		[JsonIgnore]
		public bool? IsValid { get; init; }

		protected abstract bool Validate();
	}
}