using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.IO.FileSystemTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string TempoContainerName = "tempo";
    readonly AbsolutePath TempoData = RootDirectory / "tempo-data";

    public Target RunTempo => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(TempoContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(TempoContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("grafana/tempo:latest")
                    .SetName(TempoContainerName)
                    .AddPublish(
                        "3200:3200",
                        "4317:4317",
                        "4318:4318")
                    .SetArgs("-config.file=/etc/tempo.yaml")
                    .AddVolume(
                        $"{RootDirectory}/o11y-backend/tempo-config.yaml:/etc/tempo.yaml",
                        $"{RootDirectory}/o11y-backend/tempo-overrides.yaml:/etc/overrides.yaml")
                    // $"{RootDirectory}/tempo-data:/tmp/tempo"
                    .SetHealthInterval("5s")
                    .SetHealthRetries(10)
                    .SetHealthCmd("wget --no-verbose --tries=1 --spider http://localhost:3200/status || exit 1")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });

    public Target StopTempo => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(TempoContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(TempoContainerName);

            TryDockerStop(settings);
        });

    public Target CleanTempo => _ => _
        .DependsOn(StopTempo)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(TempoContainerName);

            TryDockerRm(settings);

            EnsureCleanDirectory(TempoData);
        });
}