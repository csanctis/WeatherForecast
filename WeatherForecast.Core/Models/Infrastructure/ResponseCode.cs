using System.Diagnostics.CodeAnalysis;

namespace WeatherForecast.Core.Models.Infrastructure
{
	[ExcludeFromCodeCoverage]
	public class ResponseCode
	{
		public ResponseCode(string code, string message)
		{
			Code = code;
			Message = message;
		}

		public string Code { get; set; }
		public string Message { get; set; }
	}
}