using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using WeatherForecast.Core.Models.Infrastructure;

namespace WeatherForecast.Core.Services
{
	public interface IContextService
	{
		void Populate(HttpContext context);

		ContextBase GetContext();
	}

	[ExcludeFromCodeCoverage]
	public class ContextService : IContextService
	{
		private readonly ILogger<ContextService> _logger;
		private Context _contextModel;

		public ContextService(ILogger<ContextService> logger)
		{
			_logger = logger;
		}

		public void Populate(HttpContext context)
		{
			_contextModel = new Context(context);
			_logger.LogTrace("Context model has been populated");
		}

		public ContextBase GetContext()
		{
			return _contextModel;
		}
	}
}
