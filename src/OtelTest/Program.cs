using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddOpenTelemetry(options =>
{
    options.AddConsoleExporter();
#pragma warning disable S1075
#pragma warning disable S1075
    options.AddOtlpExporter(configure =>
    {
        configure.Endpoint = new Uri("http://localhost:5555");
        configure.Protocol = OtlpExportProtocol.Grpc;
    });
#pragma warning restore S1075
#pragma warning restore S1075
});

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