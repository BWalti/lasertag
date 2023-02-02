using Nuke.Common;
using Nuke.Common.IO;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;


// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    AbsolutePath IotOtaEngineDirectory => SourceDirectory / "Lasertag.OtaEngine";

    public Target CollectPeFiles => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            MSBuild(IotOtaEngineDirectory);
        });
}