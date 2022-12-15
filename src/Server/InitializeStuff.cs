using Lasertag.Manager.Server;
using Microsoft.Extensions.Hosting;

namespace Server;

public class InitializeStuff : IHostedService
{
    readonly IGrainFactory _grainFactory;

    public InitializeStuff(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var serverGrain = _grainFactory.GetGrain<IServerGrain>(0);
        await serverGrain.DoIt();
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}