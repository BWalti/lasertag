using Newtonsoft.Json;

namespace Lasertag.Builder.DockerModels;

public class ExposedPorts
{
    [JsonProperty("5432/tcp")] public PortBinding? _PortBinding { get; set; }
}