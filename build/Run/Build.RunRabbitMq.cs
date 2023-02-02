using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string RabbitMqContainerName = "rabbitmq";

    public Target RunMq => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(RabbitMqContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(RabbitMqContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("rabbitmq:3-management")
                    .SetName(RabbitMqContainerName)
                    .SetEnv("RABBITMQ_DEFAULT_USER=demo", "RABBITMQ_DEFAULT_PASS=demo")
                    .AddPublish("5672:5672", "15672:15672")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });


    public Target StopMq => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(RabbitMqContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(RabbitMqContainerName);

            TryDockerStop(settings);
        });

    public Target CleanMq => _ => _
        .DependsOn(StopMq)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(RabbitMqContainerName);

            TryDockerRm(settings);
        });
}