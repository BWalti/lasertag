namespace Lasertag.Builder.DockerModels;

public class RestartPolicy
{
    public string? Name { get; set; }
    public int MaximumRetryCount { get; set; }
}