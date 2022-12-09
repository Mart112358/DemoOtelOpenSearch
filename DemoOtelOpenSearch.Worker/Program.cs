using DemoOtelOpenSearch.Worker;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHttpClient<Worker>();
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();