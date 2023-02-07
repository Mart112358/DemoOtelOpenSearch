using System.Diagnostics;
using System.Diagnostics.Metrics;
using DemoOtelOpenSearch.API.Controllers;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Formatting.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<WeatherForecastController>();


// Define some important constants to initialize tracing with
const string serviceName = "DemoOtelOpenSearch.API";
const string serviceVersion = "1.0.0";
var activitySource = new ActivitySource(serviceName);

var appResourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

builder.Logging.ClearProviders();
builder.Host.UseSerilog((_, loggerConfiguration) =>
{
    loggerConfiguration
        .Enrich.FromLogContext()
        .WriteTo.Console(new JsonFormatter());
});


// builder.Logging.AddOpenTelemetry(openTelemetryLoggerOptions =>
// {
//     openTelemetryLoggerOptions.IncludeScopes = true;
//     openTelemetryLoggerOptions.IncludeFormattedMessage = true;
//     openTelemetryLoggerOptions.ParseStateValues = true;
//     
//     openTelemetryLoggerOptions
//         .AddConsoleExporter()
//         .SetResourceBuilder(appResourceBuilder)
//         .AddOtlpExporter(opt =>
//         {
//             // var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:HttpEndpoint");
//             // opt.Endpoint = new Uri(endpoint!);
//             // opt.Protocol = OtlpExportProtocol.HttpProtobuf;
//             
//             var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:gRPCEndpoint");
//             opt.Endpoint = new Uri(endpoint!);
//             opt.Protocol = OtlpExportProtocol.Grpc;            
//         });
// });

builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .AddSource(activitySource.Name)
        .SetResourceBuilder(appResourceBuilder)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(opt =>
        {
            var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:gRPCEndpoint");
            opt.Endpoint = new Uri(endpoint!);
            opt.Protocol = OtlpExportProtocol.Grpc;
        });
});

var meter = new Meter(serviceName);

builder.Services.AddOpenTelemetryMetrics(metricProviderBuilder =>
{
    metricProviderBuilder.SetResourceBuilder(appResourceBuilder);
    metricProviderBuilder.AddHttpClientInstrumentation();
    metricProviderBuilder.AddAspNetCoreInstrumentation();
    metricProviderBuilder.AddMeter(meter.Name);
    metricProviderBuilder.AddOtlpExporter(opt =>
    {
        var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:gRPCEndpoint");
        opt.Endpoint = new Uri(endpoint!);
        opt.Protocol = OtlpExportProtocol.Grpc;
    });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();