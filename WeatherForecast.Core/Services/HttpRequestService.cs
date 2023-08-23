using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WeatherForecast.Core.Services
{
	public interface IHttpRequestService
	{
#nullable enable

		Task<TResponse> Post<TRequest, TResponse>(string url, TRequest request, Dictionary<string, string>? headers = null, string? authorization = null)
			   where TResponse : IHttpResponse, new();

		Task<TResponse> Get<TResponse>(string url, Dictionary<string, string>? headers = null, string? authorization = null)
	   where TResponse : IHttpResponse, new();

#nullable disable
	}

	public interface IHttpResponse
	{
		bool IsSuccessful { get; set; }
		string Message { get; set; }
	}

	public class HttpRequestService : IHttpRequestService
	{
		private readonly ILogger<HttpRequestService> _logger;
		private readonly HttpClient _httpClientFactory;
		private readonly JsonSerializerOptions _jsonOptions;

		public HttpRequestService(ILogger<HttpRequestService> logger, HttpClient httpClientFactory)
		{
			_logger = logger;
			_httpClientFactory = httpClientFactory;

			_jsonOptions = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNameCaseInsensitive = true
			};

			_jsonOptions.Converters.Add(new JsonStringEnumConverter());
		}

		public Task<TResponse> Get<TResponse>(string url, Dictionary<string, string>? headers = null, string? authorization = null)
			where TResponse : IHttpResponse, new() => Get<TResponse>(new Uri(url), headers, authorization);

		public Task<TResponse> Post<TRequest, TResponse>(string url, TRequest request, Dictionary<string, string>? headers = null, string? authorization = null)
			where TResponse : IHttpResponse, new() => Post<TRequest, TResponse>(new Uri(url), request, headers, authorization);

		private async Task<TResponse> Get<TResponse>(Uri uri, Dictionary<string, string>? headers = null, string? authorization = null, JsonSerializerOptions jsonSerializerOptions = null)
			where TResponse : IHttpResponse, new()
		{
			var response = default(TResponse);
			var sw = Stopwatch.StartNew();

			try
			{
				using var message = CreateGetRequest(uri, HttpMethod.Get, headers, authorization, jsonSerializerOptions);
				response = await GetJsonResponse<TResponse>(message);
			}
			catch (AggregateException ae)
			{
				foreach (var e in ae.Flatten().InnerExceptions)
				{
					_logger.LogError(e, $"Task error in {nameof(HttpRequestService)}.{nameof(Post)}");
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Exception happened in {nameof(HttpRequestService)}.{nameof(Post)}");
			}

			_logger.LogInformation("POST to {url} end. Time taken: {httpElapsedMilliseconds} ms", uri, sw.ElapsedMilliseconds);

			return response ?? new();
		}

		private HttpRequestMessage CreateGetRequest(Uri uri, HttpMethod method, Dictionary<string, string>? headers = null, string? authorization = null, JsonSerializerOptions jsonSerializerOptions = null)
		{
			var message = new HttpRequestMessage(method, uri);
			JsonSerializerOptions jsonOptions = jsonSerializerOptions ?? _jsonOptions;

			if (headers != null)
			{
				foreach (var header in headers)
				{
					message.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
			}

			if (!string.IsNullOrEmpty(authorization))
			{
				var sections = authorization.Split(' ');
				if (sections.Length == 1)
				{
					message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorization);
				}
				else
				{
					message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(sections[0], sections[1]);
				}
			}

			return message;
		}

		private async Task<TResponse> Post<TRequest, TResponse>(Uri uri, TRequest request, Dictionary<string, string>? headers = null, string? authorization = null, JsonSerializerOptions jsonSerializerOptions = null)
			where TResponse : IHttpResponse, new()
		{
			var response = default(TResponse);
			var sw = Stopwatch.StartNew();

			try
			{
				using var message = CreateJsonMessage(uri, HttpMethod.Post, request, headers, authorization, jsonSerializerOptions);
				response = await GetJsonResponse<TResponse>(message);
			}
			catch (AggregateException ae)
			{
				foreach (var e in ae.Flatten().InnerExceptions)
				{
					_logger.LogError(e, $"Task error in {nameof(HttpRequestService)}.{nameof(Post)}");
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Exception happened in {nameof(HttpRequestService)}.{nameof(Post)}");
			}

			_logger.LogInformation("POST to {url} end. Time taken: {httpElapsedMilliseconds} ms", uri, sw.ElapsedMilliseconds);

			return response ?? new();
		}

		private HttpRequestMessage CreateJsonMessage<TRequest>(Uri uri, HttpMethod method, TRequest? request, Dictionary<string, string>? headers = null, string? authorization = null, JsonSerializerOptions jsonSerializerOptions = null)
		{
			var message = new HttpRequestMessage(method, uri);
			JsonSerializerOptions jsonOptions = jsonSerializerOptions ?? _jsonOptions;

			if (request != null)
			{
				message.Content = new StringContent(JsonSerializer.Serialize(request, jsonOptions), System.Text.Encoding.UTF8, "application/json");
			}

			if (headers != null)
			{
				foreach (var header in headers)
				{
					message.Headers.TryAddWithoutValidation(header.Key, header.Value);
				}
			}

			if (!string.IsNullOrEmpty(authorization))
			{
				var sections = authorization.Split(' ');
				if (sections.Length == 1)
				{
					message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(authorization);
				}
				else
				{
					message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(sections[0], sections[1]);
				}
			}

			return message;
		}

		private async Task<TResponse> GetJsonResponse<TResponse>(HttpRequestMessage message)
			where TResponse : IHttpResponse, new()
		{
			try
			{
				using var httpResponse = await _httpClientFactory.SendAsync(message);
				var body = await httpResponse.Content.ReadAsStringAsync();

				if (string.IsNullOrEmpty(body))
				{
					body = "{}";
				}

				if (body.StartsWith("["))
				{
					char[] charArray = body.ToCharArray();
					charArray = charArray.Skip(1).Take(charArray.Length - 2).ToArray();
					body = new string(charArray);
				}

				var response = JsonSerializer.Deserialize<TResponse>(body, _jsonOptions) ?? new();

				_logger.LogInformation("{httpMethod} to {url} with result {httpStatusCode}", message.Method, message.RequestUri, httpResponse.StatusCode);
				var statusCode = Convert.ToInt32(httpResponse.StatusCode);

				if (statusCode >= 200 && statusCode < 300)
				{
					response.IsSuccessful = true;
				}

				return response;
			}
			catch (Exception)
			{
				throw;
			}
		}

		private async Task<bool> PostWithoutResponse<TRequest>(Uri uri, TRequest request, Dictionary<string, string>? headers = null, string? authorization = null, JsonSerializerOptions jsonSerializerOptions = null)
		{
			var response = false;
			var sw = Stopwatch.StartNew();

			try
			{
				using var message = CreateJsonMessage(uri, HttpMethod.Post, request, headers, authorization, jsonSerializerOptions);
				response = await IsResponseSuccessfully(message);
			}
			catch (AggregateException ae)
			{
				foreach (var e in ae.Flatten().InnerExceptions)
				{
					_logger.LogError(e, $"Task error in {nameof(HttpRequestService)}.{nameof(PostWithoutResponse)}");
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, $"Exception happened in {nameof(HttpRequestService)}.{nameof(PostWithoutResponse)}");
			}

			_logger.LogInformation("POST to {url} end. Time taken: {httpElapsedMilliseconds} ms", uri, sw.ElapsedMilliseconds);

			return response;
		}

		private async Task<bool> IsResponseSuccessfully(HttpRequestMessage message)
		{
			try
			{
				using var httpResponse = await _httpClientFactory.SendAsync(message);
				_logger.LogInformation("{httpMethod} to {url} with result {httpStatusCode}", message.Method, message.RequestUri, httpResponse.StatusCode);
				return httpResponse.IsSuccessStatusCode;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}