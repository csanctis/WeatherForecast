using Microsoft.AspNetCore.Http;

namespace WeatherForecast.Core.Models.Infrastructure
{
	public sealed record Context : ContextBase
	{
		private const string Token_XApiKey = "X-Api-Key";

		public Context() { }

		public Context(HttpContext context)
		{
			var header = context?.Request?.Headers?
								.Where(x => !String.Equals(x.Key, Token_XApiKey, StringComparison.OrdinalIgnoreCase))
								.ToDictionary(a => a.Key, a => string.Join(";", a.Value), StringComparer.InvariantCultureIgnoreCase);

			Header = header;
			IsValid = Validate();
		}

		protected override bool Validate()
		{
			// Validations to the context go here
			return true;
		}
	}
}