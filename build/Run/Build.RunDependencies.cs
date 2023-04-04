using Nuke.Common;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    public Target RunMonitoring => _ => _
        .DependsOn(RunOtelCollector, RunPrometheus, RunTempo, RunGrafana, RunLoki);

    public Target StopMonitoring => _ => _
        .DependsOn(StopOtelCollector, StopPrometheus, StopTempo, StopGrafana, StopLoki);

    public Target CleanMonitoring => _ => _
        .DependsOn(CleanOtelCollector, CleanPrometheus, CleanTempo, CleanGrafana, CleanLoki);

    public Target RunApplicationDependencies => _ => _
        .DependsOn(RunPostgres, RunEmqx);

    public Target StopApplicationDependencies => _ => _
        .DependsOn(StopPostgres, StopEmqx);

    public Target CleanApplicationDependencies => _ => _
        .DependsOn(CleanPostgres, CleanEmqx);

    public Target RunDependencies => _ => _
        .DependsOn(RunMonitoring, RunApplicationDependencies);

    public Target StopDependencies => _ => _
        .DependsOn(StopMonitoring, StopApplicationDependencies);

    public Target CleanDependencies => _ => _
        .DependsOn(CleanMonitoring, CleanApplicationDependencies);
}