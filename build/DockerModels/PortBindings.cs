using Newtonsoft.Json;

namespace Lasertag.Builder.DockerModels;

#pragma warning disable MA0016
public class PortBindings
{
    [JsonProperty("5432/tcp")] public List<PortBinding>? _PortBinding { get; set; }
}