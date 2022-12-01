using Newtonsoft.Json;

namespace Lasertag.Builder.DockerModels;

public class Volumes
{
    [JsonProperty("/var/lib/postgresql/data")] public VarLibPostgresqlData? VarLibPostgresqlData { get; set; }
}