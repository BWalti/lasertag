using Nuke.Common;
using Nuke.Common.IO;
using Nuke.Common.Tooling;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Builder;

partial class Build
{
    [PathVariable] Tool Yarn = default!;

    AbsolutePath ClientDirectory => RootDirectory / "Client";

    public Target GenerateTypings => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNet(
                "swagger tofile --output .\\Client\\api-spec.json .\\src\\Admin.Api\\bin\\Debug\\net6.0\\Admin.Api.dll v1");
            Yarn("run generate-typings", ClientDirectory);
        });
}