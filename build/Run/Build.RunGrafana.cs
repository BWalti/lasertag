using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string GrafanaContainerName = "grafana";

    public Target RunGrafana => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(GrafanaContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(GrafanaContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("grafana/grafana:latest")
                    .SetName(GrafanaContainerName)
                    .AddPublish("3000:3000")
                    .AddLink(TempoContainerName, PrometheusContainerName)
                    .AddVolume(
                        $"{RootDirectory}/o11y-backend/grafana-bootstrap.ini:/etc/grafana/grafana.ini",
                        $"{RootDirectory}/o11y-backend/grafana-datasources.yaml:/etc/grafana/provisioning/datasources/datasources.yaml")
                    .SetEnv(
                        "GF_AUTH_ANONYMOUS_ENABLED=true",
                        "GF_AUTH_ANONYMOUS_ORG_ROLE=Admin",
                        "GF_AUTH_DISABLE_LOGIN_FORM=true")
                    .SetHealthInterval("5s")
                    .SetHealthRetries(10)
                    .SetHealthCmd("wget --no-verbose --tries=1 --spider http://localhost:3000 || exit 1\r\n")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });

    public Target StopGrafana => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(GrafanaContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(GrafanaContainerName);

            TryDockerStop(settings);
        });

    public Target CleanGrafana => _ => _
        .DependsOn(StopGrafana)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(GrafanaContainerName);

            TryDockerRm(settings);
        });
}