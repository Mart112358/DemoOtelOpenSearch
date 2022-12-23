using Microsoft.AspNetCore.Mvc;

namespace DemoOtelOpenSearch.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public WeatherForecastController(ILogger<WeatherForecastController> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    [HttpGet("hello")]
    public string SayHello()
    {
        return "Hello World";
    }
    

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        using var scope = _logger.BeginScope("{Id}", 
            Guid.NewGuid().ToString("N"));

        var sayHelloUrl = _configuration.GetValue<string>("SayHelloAPI");
        var result = await _httpClient
            .GetStringAsync(sayHelloUrl);
        
        _logger.LogInformation("Say Hello Result : {result}", result);
        
        var forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();

        _logger.LogError(
            "WeatherForecasts generated {count}: {forecasts}",
            forecasts.Length,
            forecasts);

        return forecasts;
    }
}