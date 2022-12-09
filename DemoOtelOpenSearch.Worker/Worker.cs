namespace DemoOtelOpenSearch.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger,
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var requestUri = _configuration.GetValue<string>("DemoOtelOpenSearchAPI");
            var httpResponseMessage = await _httpClient
                .GetAsync(requestUri, stoppingToken);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var content = await httpResponseMessage.Content.ReadAsStringAsync(stoppingToken);
                _logger.LogInformation(content);                
            }
            
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(1000, stoppingToken);
        }
    }
}