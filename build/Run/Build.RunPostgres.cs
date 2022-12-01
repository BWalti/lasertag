using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string PostgresContainerName = "postgres";

    public Target RunPostgres => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(PostgresContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(PostgresContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("timescale/timescaledb:latest-pg13")
                    .SetName(PostgresContainerName)
                    .SetEnv("POSTGRES_PASSWORD=demo", "POSTGRES_DB=demo", "POSTGRES_USER=demo")
                    .AddPublish("5432:5432")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });

    public Target StopPostgres => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(PostgresContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(PostgresContainerName);

            TryDockerStop(settings);
        });

    public Target CleanPostgres => _ => _
        .DependsOn(StopPostgres)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(PostgresContainerName);

            TryDockerRm(settings);
        });
}