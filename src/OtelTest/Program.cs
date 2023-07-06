using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
#pragma warning disable S1075
    options.AddOtlpExporter(configure =>
    {
        configure.Endpoint = new Uri("http://localhost:5555");
        configure.Protocol = OtlpExportProtocol.Grpc;
    });
#pragma warning restore S1075
});

builder.Services.AddOpenTelemetry()
    .WithMetrics(providerBuilder =>
        providerBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault())
            .AddRuntimeInstrumentation()
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddPrometheusExporter()
            .AddOtlpExporter(c =>
            {
#pragma warning disable S1075
                c.Endpoint = new Uri("http://localhost:4312");
#pragma warning restore S1075
            }));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();