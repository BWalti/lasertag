using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Lasertag.IoT.Simulator;

public class IotSimulatorHostedService : IHostedService
{
    readonly ILogger<IotSimulatorHostedService> _logger;
    readonly IotStateMachine _stateMachine;

    public IotSimulatorHostedService(IotStateMachine stateMachine, ILogger<IotSimulatorHostedService> logger)
    {
        _stateMachine = stateMachine;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting IoT Simulator...");
        await _stateMachine.Initialize();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping IoT Simulator...");
        return Task.CompletedTask;
    }
}