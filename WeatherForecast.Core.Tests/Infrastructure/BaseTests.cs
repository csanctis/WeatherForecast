using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using WeatherForecast.Core.Models.Infrastructure;

namespace WeatherForecasts.Core.Tests.Infrastructure
{
	public abstract class BaseTests
	{
		protected const string StringRandomToken = "#STRING#";
		protected static readonly Random rd = new();

		protected readonly JsonSerializerOptions _jsonOptions;

		public BaseTests()
		{
			_jsonOptions = new JsonSerializerOptions()
			{
				DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
				PropertyNameCaseInsensitive = true
			};

			_jsonOptions.Converters.Add(new JsonStringEnumConverter());
		}

		protected static void SetProperty(string compoundProperty, object target, object value)
		{
			if (value != null && value is string && value.ToString().StartsWith(StringRandomToken))
			{
				int stringLength = Convert.ToInt32(value.ToString().Replace(StringRandomToken, string.Empty));
				value = CreateString(stringLength);
			}

			string[] bits = compoundProperty.Split('.');
			for (int i = 0; i < bits.Length - 1; i++)
			{
				var propertyToGet = target.GetType().GetProperty(bits[i]);
				target = propertyToGet.GetValue(target, null);
			}
			var propertyToSet = target.GetType().GetProperty(bits.Last());
			propertyToSet.SetValue(target, value, null);
		}

		protected static string CreateString(int stringLength)
		{
			const string allowedChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz0123456789!@$?_-";
			char[] chars = new char[stringLength];

			for (int i = 0; i < stringLength; i++)
			{
				chars[i] = allowedChars[rd.Next(0, allowedChars.Length)];
			}

			return new string(chars);
		}

		protected static Context GetValidContextModel()
		{
			return new Context()
			{
				IsValid = true
			};
		}

		protected enum JsonFiles
		{
			WeatherDataResponse
		}

		protected static T LoadJson<T>(string jsonContent)
		{
			return JsonSerializer.Deserialize<T>(jsonContent, _serializeOptions);
		}

		protected static T LoadJson<T>(JsonFiles jsonFile)
		{
			var jsonString = LoadJson(jsonFile);
			return LoadJson<T>(jsonString);
		}

		protected static string LoadJson(JsonFiles jsonFile)
		{
			var resourceName = $"WeatherForecast.Core.Tests.Infrastructure.Resources.{jsonFile}.json";
			return LoadJson(resourceName);
		}

		protected static string LoadJson(string resourceName)
		{
			using StreamReader reader = new(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName));
			var fileContent = reader.ReadToEnd();
			if (string.IsNullOrEmpty(fileContent))
			{
				throw new Exception($"Json file ({resourceName}) was not found or it is empty!");
			}
			return fileContent;
		}

		private static readonly JsonSerializerOptions _serializeOptions = new(JsonSerializerDefaults.Web)
		{
			Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
			DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
		};
	}
}