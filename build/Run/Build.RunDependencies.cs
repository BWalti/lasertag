using Nuke.Common;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    public Target RunDependencies => _ => _
        .DependsOn(RunPrometheus, RunTempo, RunGrafana, RunMq, RunPostgres, RunOtelCollector, RunLoki);

    public Target StopDependencies => _ => _
        .DependsOn(StopMq, StopPostgres, StopOtelCollector, StopPrometheus, StopTempo, StopGrafana, StopLoki);

    public Target CleanDependencies => _ => _
        .DependsOn(CleanMq, CleanPostgres, CleanOtelCollector, CleanTempo, CleanPrometheus, CleanGrafana, CleanLoki);
}