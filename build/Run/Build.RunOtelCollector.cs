using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{

    const string OtelContainerName = "otel-collector";

    public Target RunOtelCollector => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(OtelContainerName))
        .DependsOn(RunTempo)
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(OtelContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("otel/opentelemetry-collector:latest")
                    .SetName(OtelContainerName)
                    .AddPublish(
                        "5555:5555",
                        "8765:8765")
                    .AddLink(TempoContainerName)
                    .SetArgs("--config=/etc/collector-config.yaml")
                    .AddVolume($"{RootDirectory}/o11y-backend/collector-config-local.yaml:/etc/collector-config.yaml")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });

    public Target StopOtelCollector => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(OtelContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(OtelContainerName);

            TryDockerStop(settings);
        });

    public Target CleanOtelCollector => _ => _
        .DependsOn(StopOtelCollector)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(OtelContainerName);

            TryDockerRm(settings);
        });
}