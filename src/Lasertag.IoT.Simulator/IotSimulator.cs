using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lasertag.IoT.Simulator;

public class IotSimulator : IHostedService
{
    readonly IotStateMachine _stateMachine;
    readonly ILogger<IotSimulator> _logger;

    public IotSimulator(IotStateMachine stateMachine, ILogger<IotSimulator> logger)
    {
        _stateMachine = stateMachine;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _stateMachine.Initialize();
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}