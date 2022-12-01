using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string PrometheusContainerName = "prometheus";

    public Target RunPrometheus => _ => _
        .DependsOn(RunOtelCollector)
        .OnlyWhenDynamic(() => !DockerIsRunning(PrometheusContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(PrometheusContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("prom/prometheus:latest")
                    .SetName(PrometheusContainerName)
                    .AddPublish("9090:9090")
                    .AddLink(OtelContainerName)
                    .SetArgs(
                        "--config.file=/etc/prometheus.yaml",
                        "--web.enable-remote-write-receiver",
                        "--enable-feature=exemplar-storage")
                    .AddVolume($"{RootDirectory}/o11y-backend/prometheus.yaml:/etc/prometheus.yaml")
                    .SetHealthInterval("5s")
                    .SetHealthRetries(10)
                    .SetHealthCmd("wget --no-verbose --tries=1 --spider http://localhost:9090/status || exit 1")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });

    public Target StopPrometheus => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(PrometheusContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(PrometheusContainerName);

            TryDockerStop(settings);
        });

    public Target CleanPrometheus => _ => _
        .DependsOn(StopPrometheus)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(PrometheusContainerName);

            TryDockerRm(settings);
        });
}