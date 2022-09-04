﻿using Newtonsoft.Json;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Docker;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lasertag.Builder.DockerModels;
using Serilog;
using static Nuke.Common.Tools.Docker.DockerTasks;

namespace Lasertag.Builder;
public partial class Build
{
    string GetImageTag(string projectName) =>
        $"{projectName.Replace("Lasertag.", string.Empty).Replace(".", "-").ToLower()}:{ImageTag}";

    void ProcessDockerTemplate(string projectName, string template)
    {
        var rendered = template
            .Replace("{APPNAME}", projectName);

        File.WriteAllText(Path.Combine(OutputDirectory, projectName, "Dockerfile"), rendered);
    }

    void TryDockerRm(DockerRmSettings settings)
    {
        try
        {
            DockerRm(settings);
        }
        catch (ProcessException e)
        {
            // do nothing!
            Log.Logger.Information("Process Exception happened: {Message}", e.Message);
        }
    }

    void TryDockerStop(DockerStopSettings settings)
    {
        try
        {
            DockerStop(settings);
        }
        catch (ProcessException e)
        {
            // do nothing!
            Log.Logger.Information("Process Exception happened: {Message}", e.Message);
        }
    }

    bool DockerIsRunning(string containerName)
    {
        var settings = new DockerInspectSettings()
            .SetNames(containerName);

        try
        {
            var result = DockerInspect(settings)
                .Where(o => o.Type == OutputType.Std)
                .Aggregate(new StringBuilder(""),
                    (builder, output) => builder.AppendLine(output.Text), builder => builder.ToString());

            var inspectionResults = JsonConvert.DeserializeObject<List<InspectResult>>(result);
            if (inspectionResults == null || !inspectionResults.Any())
            // nothing running:
            {
                return false;
            }

            return inspectionResults.First().State!.Running;
        }
        catch (ProcessException)
        {
            return false;
        }
    }

    bool TryDockerStartIfStopped(string containerName)
    {
        if (DockerIsStopped(containerName))
        {
            var settings = new DockerStartSettings()
                .SetContainers(containerName);

            DockerStart(settings);

            return true;
        }

        return false;
    }

    bool DockerIsStopped(string containerName)
    {
        var settings = new DockerInspectSettings()
            .SetNames(containerName);
        try
        {
            var stdOutput = DockerInspect(settings)
                .Where(o => o.Type == OutputType.Std)
                .Aggregate(new StringBuilder(""),
                    (builder, output) => builder.AppendLine(output.Text), builder => builder.ToString());

            var inspectionResults = JsonConvert.DeserializeObject<List<InspectResult>>(stdOutput);
            if (!inspectionResults.Any())
            // non existing, thus "not stopped":
            {
                return false;
            }

            var first = inspectionResults.First();
            return first.State!.Running && first.State.Status == "exited";
        }
        catch (ProcessException)
        {
            return false;
        }
    }
}
