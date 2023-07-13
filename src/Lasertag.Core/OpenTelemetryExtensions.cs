using System.Diagnostics;

namespace Lasertag.Core;

public static class OpenTelemetryExtensions
{
    internal static readonly ActivitySource ActivitySource = new("Lasertag.Core");
}