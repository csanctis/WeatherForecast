using Microsoft.AspNetCore.Http.Json;
using WeatherForecast.Core.Extensions;
using WeatherForecast.Core.Models.Config;
using WeatherForecast.Core.Services;
using WeatherForecast.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add AWS Lambda support. When application is run in Lambda Kestrel is swapped out as the web server with Amazon.Lambda.AspNetCoreServer. This
// package will act as the webserver translating request and responses between the Lambda event source and ASP.NET Core.
builder.Services.AddAWSLambdaHosting(LambdaEventSource.RestApi);

// Configure JSON options
builder.Services.Configure<JsonOptions>(options =>
{
	options.SerializerOptions.ConfigSerializerOptions();
});

// Config sources
var environmentName = builder.Environment.EnvironmentName;
var configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
	.AddEnvironmentVariables()
	.Build();

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<AppConfig>(configuration.GetSection(AppConfig.SectionName));
builder.Services.AddScoped<IContextService, ContextService>();
builder.Services.AddHttpClient<IHttpRequestService, HttpRequestService>();
builder.Services.AddCoreServices();

// Configure caching
builder.Services.AddOutputCache(options =>
{
	options.AddBasePolicy(builder =>
		builder.Expire(TimeSpan.FromMinutes(1)));
	options.AddPolicy("Expire5", builder =>
		builder.Expire(TimeSpan.FromMinutes(5)));
	options.AddPolicy("Expire10", builder =>
		builder.Expire(TimeSpan.FromMinutes(10)));
});
var app = builder.Build();

//app.UseExceptionHandler(builder => builder.HandleException(loggerFactory));
app.UseContextMiddleWare();
app.UseLoggingMiddleWare();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseOutputCache(); // Enable caching from now on
app.UseEndpointExtensions();

app.Run();