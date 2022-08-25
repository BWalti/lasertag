using GrainInterfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

var host = Host.CreateDefaultBuilder(args);
host.ConfigureServices(services =>
{
    services.AddLogging();
    services.AddOrleansClient(builder =>
    {
        builder.UseLocalhostClustering();
    });
});

host.ConfigureLogging(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning)   // generic host lifecycle messages
        .AddFilter("Orleans", LogLevel.Information)     // suppress status dumps
        .AddFilter("Runtime", LogLevel.Warning)     // also an Orleans prefix
        .AddDebug()                                 // VS Debug window
        .AddConsole();
});

var app = host.Build();

await app.StartAsync();

var client = app.Services.GetRequiredService<IClusterClient>();

var logConsistent = client.GetGrain<ISomeMartenJournaledGrain>(Guid.Parse("{251014C3-05AA-45B5-B3D3-D3CFD539E548}"));
await logConsistent.RaiseAmount(2);
await logConsistent.RaiseAmount(2);
await logConsistent.DecreaseAmount(1);

var amount = await logConsistent.GetAmount();
Console.WriteLine(amount);

Console.ReadLine();
await app.StopAsync();