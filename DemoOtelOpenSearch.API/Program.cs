using System.Diagnostics;
using System.Diagnostics.Metrics;
using DemoOtelOpenSearch.API.Controllers;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient<WeatherForecastController>();


builder.Logging.ClearProviders();

// Define some important constants to initialize tracing with
const string serviceName = "DemoOtelOpenSearch.API";
const string serviceVersion = "1.0.0";
var activitySource = new ActivitySource(serviceName);

var appResourceBuilder = ResourceBuilder.CreateDefault()
    .AddService(serviceName: serviceName, serviceVersion: serviceVersion);

builder.Logging.AddOpenTelemetry(openTelemetryLoggerOptions =>
{
    openTelemetryLoggerOptions.IncludeScopes = true;
    openTelemetryLoggerOptions.IncludeFormattedMessage = true;
    openTelemetryLoggerOptions.ParseStateValues = true;
    
    openTelemetryLoggerOptions
        .AddConsoleExporter()
        .AddOtlpExporter(opt =>
        {
            var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:gRPCEndpoint");
            opt.Endpoint = new Uri(endpoint!);
            opt.Protocol = OtlpExportProtocol.Grpc;
        })
        .SetResourceBuilder(appResourceBuilder);
});

// Configure to send data via the OTLP exporter.
// By default, it will send to port 4318, which the collector is listening on.
builder.Services.AddOpenTelemetryTracing(tracerProviderBuilder =>
{
    tracerProviderBuilder
        .AddConsoleExporter()
        .AddOtlpExporter(opt =>
        {
            var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:gRPCEndpoint");
            opt.Endpoint = new Uri(endpoint!);
            opt.Protocol = OtlpExportProtocol.Grpc;
        })
        .AddSource(activitySource.Name)
        .SetResourceBuilder(appResourceBuilder)
        .AddHttpClientInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddSqlClientInstrumentation();
});

var meter = new Meter(serviceName);
builder.Services.AddOpenTelemetryMetrics(metricProviderBuilder =>
{
    metricProviderBuilder
        //.AddConsoleExporter()
        .AddOtlpExporter(opt =>
        {
            var endpoint = builder.Configuration.GetValue<string>("OpenTelemetry:gRPCEndpoint");
            opt.Endpoint = new Uri(endpoint!);
            opt.Protocol = OtlpExportProtocol.Grpc;
        })
        .AddMeter(meter.Name)
        .SetResourceBuilder(appResourceBuilder)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation();
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