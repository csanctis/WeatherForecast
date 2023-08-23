using System.Diagnostics.CodeAnalysis;
using WeatherForecast.Core.Models.Infrastructure;

namespace WeatherForecast.Core.Extensions
{
	[ExcludeFromCodeCoverage]
	public static class ServiceResponseExtensions
	{
		public static ServiceResponse<T> With<T>(this ServiceResponse<T> _, T value)
		{
			return new ServiceResponse<T>(value);
		}

		public static ServiceResponse<T> WithError<T, R>(this ServiceResponse<T> _, ServiceResponse<R> serviceResponse)
		{
			return _.WithError(serviceResponse.ErrorType, serviceResponse.ResponseCodes);
		}

		public static ServiceResponse<T> WithError<T>(this ServiceResponse<T> _, ServiceResponseErrorType? errorType, List<ResponseCode> errors = null)
		{
			return new ServiceResponse<T>(errorType ?? ServiceResponseErrorType.SystemError, errors);
		}
	}
}