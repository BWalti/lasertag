using Admin.Api;
using Admin.Api.Extensions;
using Lasertag.Api;
using OpenTelemetry.Exporter;

var builder = WebApplication
    .CreateBuilder(args)
    .UseDefaultInfrastructure()
    .UseSwaggerGeneratorHack(args);

builder.AddOpenTelemetry();

builder.Services.AddTransient(provider =>
{
    var clusterClient = provider.GetRequiredService<IClusterClient>();
    return clusterClient.GetGrain<IGameCommands>(0);
});

builder.Services.AddTransient(provider =>
{
    var clusterClient = provider.GetRequiredService<IClusterClient>();
    return clusterClient.GetGrain<IGameRoundCommands>(0);
});

builder.Services.AddTransient(provider =>
{
    var clusterClient = provider.GetRequiredService<IClusterClient>();
    return clusterClient.GetGrain<IGameRoundQueries>(0);
});

builder.Services.AddOptions();
builder.Services.Configure<OtlpExporterOptions>(builder.Configuration.GetSection("OtlpExporter"));

var app = builder
    .Build()
    .UseDevelopmentDefaults();

app.MapGameEndpoints();
app.MapGameRoundEndpoints();
app.MapGameStatisticsEndpoints();

app.Run();
