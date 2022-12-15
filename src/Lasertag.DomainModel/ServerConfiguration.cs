namespace Lasertag.DomainModel;

[GenerateSerializer]
public class ServerConfiguration
{
    [Id(0)] public string Name { get; set; } = string.Empty;

    [Id(0)] public string MqttConnection { get; set; } = string.Empty;
}