using Nuke.Common;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    public Target RunDependencies => _ => _
        .DependsOn(RunMq, RunPostgres);

    public Target StopDependencies => _ => _
        .DependsOn(StopMq, StopPostgres);

    public Target CleanDependencies => _ => _
        .DependsOn(CleanMq, CleanPostgres);
}