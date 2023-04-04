using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.IO.FileSystemTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string LokiContainerName = "loki";
    readonly AbsolutePath LokiData = RootDirectory / "loki-data";

    public Target RunLoki => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(LokiContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(LokiContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("grafana/loki:latest")
                    .SetName(LokiContainerName)
                    .AddPublish(
                        "3100:3100",
                        "9096:9096")
                    .SetArgs("-config.file=/mnt/config/loki-config.yaml")
                    .AddVolume($"{RootDirectory}/o11y-backend/loki-config.yaml:/mnt/config/loki-config.yaml")
                    // , $"{RootDirectory}/loki-data:/loki"
                    .SetHealthInterval("5s")
                    .SetHealthRetries(10)
                    .SetHealthCmd("wget --no-verbose --tries=1 --spider http://localhost:3100/ready || exit 1")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });

    public Target StopLoki => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(LokiContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(LokiContainerName);

            TryDockerStop(settings);
        });

    public Target CleanLoki => _ => _
        .DependsOn(StopLoki)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(LokiContainerName);

            TryDockerRm(settings);

            EnsureCleanDirectory(LokiData);
        });
}