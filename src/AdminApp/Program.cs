using AdminApp.Data;
using EasyNetQ.ConnectionString;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
    .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
    .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
    .AddDebug() // VS Debug window
    .AddConsole();

builder.Services.AddOrleansClient(clientBuilder => { clientBuilder.UseLocalhostClustering(); });

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.RegisterEasyNetQ(resolver =>
{
    var parser = resolver.Resolve<IConnectionStringParser>();
    var configuration = resolver.Resolve<IConfiguration>();

    var connectionString = configuration["Mq:Host"];
    return parser.Parse(connectionString);
});
//builder.Services.
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();