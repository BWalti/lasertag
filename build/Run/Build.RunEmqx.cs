using Nuke.Common;
using Nuke.Common.Tools.Docker;
using static Nuke.Common.Tools.Docker.DockerTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    const string EmqxContainerName = "emqx";

    public Target RunEmqx => _ => _
        .OnlyWhenDynamic(() => !DockerIsRunning(EmqxContainerName))
        .Executes(() =>
        {
            if (!TryDockerStartIfStopped(EmqxContainerName))
            {
                var settings = new DockerRunSettings()
                    .SetImage("emqx")
                    .SetName(EmqxContainerName)
                    .SetEnv("EMQX_DASHBOARD__DEFAULT_PASSWORD=MyNewPassword123")
                    .AddPublish("1883:1883", "8083:8083", "18083:18083")
                    .SetDetach(true);

                DockerRun(settings);
            }
        });


    public Target StopEmqx => _ => _
        .OnlyWhenDynamic(() => DockerIsRunning(EmqxContainerName))
        .Executes(() =>
        {
            var settings = new DockerStopSettings()
                .SetContainers(EmqxContainerName);

            TryDockerStop(settings);
        });

    public Target CleanEmqx => _ => _
        .DependsOn(StopEmqx)
        .Executes(() =>
        {
            var settings = new DockerRmSettings()
                .SetContainers(EmqxContainerName);

            TryDockerRm(settings);
        });
}