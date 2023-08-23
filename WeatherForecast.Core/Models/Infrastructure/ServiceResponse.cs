using System.Diagnostics.CodeAnalysis;

namespace WeatherForecast.Core.Models.Infrastructure
{
	[ExcludeFromCodeCoverage]
	public readonly struct ServiceResponse<T>
	{
		private readonly T _value;

		public bool IsValid { get; }

		public ServiceResponseErrorType? ErrorType { get; }

		public List<ResponseCode> ResponseCodes { get; }

		public T Value
		{
			get
			{
				if (!IsValid)
				{
					throw new InvalidOperationException("Service response is not valid.");
				}

				return _value;
			}
		}

		public ServiceResponse(T value, List<ResponseCode> responseCodes = null)
		{
			_value = value;
			IsValid = true;
			ErrorType = null;
			ResponseCodes = responseCodes;
		}

		public ServiceResponse(ServiceResponseErrorType errorType, List<ResponseCode> errors = null)
		{
			_value = default;
			IsValid = false;
			ErrorType = errorType;
			ResponseCodes = errors ?? new List<ResponseCode>();
		}

		public static implicit operator bool(ServiceResponse<T> response)
		{
			return response.IsValid;
		}

		public static implicit operator T(ServiceResponse<T> response)
		{
			return response.Value;
		}

		public static implicit operator ServiceResponseErrorType?(ServiceResponse<T> response)
		{
			return response.ErrorType;
		}
	}

	public enum ServiceResponseErrorType
	{
		ValidationError,
		SystemError
	}
}