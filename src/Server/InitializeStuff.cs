using Lasertag.Manager.Server;
using Microsoft.Extensions.Hosting;
using Orleans;

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
        await serverGrain.DoIt().ConfigureAwait(false);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}