namespace Lasertag.Builder.DockerModels;

#pragma warning disable MA0016
public class Config
{
    public string? Hostname { get; set; }
    public string? Domainname { get; set; }
    public string? User { get; set; }
    public bool AttachStdin { get; set; }
    public bool AttachStdout { get; set; }
    public bool AttachStderr { get; set; }
    public ExposedPorts? ExposedPorts { get; set; }
    public bool Tty { get; set; }
    public bool OpenStdin { get; set; }
    public bool StdinOnce { get; set; }
    public List<string>? Env { get; set; }
    public List<string>? Cmd { get; set; }
    public string? Image { get; set; }
    public Volumes? Volumes { get; set; }
    public string? WorkingDir { get; set; }
    public List<string>? Entrypoint { get; set; }
    public object? OnBuild { get; set; }
    public Labels? Labels { get; set; }
    public string? StopSignal { get; set; }
}