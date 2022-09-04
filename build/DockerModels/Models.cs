using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lasertag.Builder.DockerModels;
#nullable enable


public class State
{
    public string? Status { get; set; }
    public bool Running { get; set; }
    public bool Paused { get; set; }
    public bool Restarting { get; set; }
    public bool OOMKilled { get; set; }
    public bool Dead { get; set; }
    public int Pid { get; set; }
    public int ExitCode { get; set; }
    public string? Error { get; set; }
    public string? StartedAt { get; set; }
    public string? FinishedAt { get; set; }
}

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

public class LogConfig
{
    public string? Type { get; set; }
    public Config? Config { get; set; }
}

public class PortBinding
{
    public string? HostIp { get; set; }
    public string? HostPort { get; set; }
}

public class PortBindings
{
    [JsonProperty("5432/tcp")] public List<PortBinding>? _PortBinding { get; set; }
}

public class RestartPolicy
{
    public string? Name { get; set; }
    public int MaximumRetryCount { get; set; }
}

public class HostConfig
{
    public object? Binds { get; set; }
    public string? ContainerIDFile { get; set; }
    public LogConfig? LogConfig { get; set; }
    public string? NetworkMode { get; set; }
    public PortBindings? PortBindings { get; set; }
    public RestartPolicy? RestartPolicy { get; set; }
    public bool AutoRemove { get; set; }
    public string? VolumeDriver { get; set; }
    public object? VolumesFrom { get; set; }
    public object? CapAdd { get; set; }
    public object? CapDrop { get; set; }
    public string? CgroupnsMode { get; set; }
    public List<object>? Dns { get; set; }
    public List<object>? DnsOptions { get; set; }
    public List<object>? DnsSearch { get; set; }
    public object? ExtraHosts { get; set; }
    public object? GroupAdd { get; set; }
    public string? IpcMode { get; set; }
    public string? Cgroup { get; set; }
    public object? Links { get; set; }
    public int OomScoreAdj { get; set; }
    public string? PidMode { get; set; }
    public bool Privileged { get; set; }
    public bool PublishAllPorts { get; set; }
    public bool ReadonlyRootfs { get; set; }
    public object? SecurityOpt { get; set; }
    public string? UTSMode { get; set; }
    public string? UsernsMode { get; set; }
    public int ShmSize { get; set; }
    public string? Runtime { get; set; }
    public List<int>? ConsoleSize { get; set; }
    public string? Isolation { get; set; }
    public int CpuShares { get; set; }
    public int Memory { get; set; }
    public int NanoCpus { get; set; }
    public string? CgroupParent { get; set; }
    public int BlkioWeight { get; set; }
    public List<object>? BlkioWeightDevice { get; set; }
    public object? BlkioDeviceReadBps { get; set; }
    public object? BlkioDeviceWriteBps { get; set; }
    public object? BlkioDeviceReadIOps { get; set; }
    public object? BlkioDeviceWriteIOps { get; set; }
    public int CpuPeriod { get; set; }
    public int CpuQuota { get; set; }
    public int CpuRealtimePeriod { get; set; }
    public int CpuRealtimeRuntime { get; set; }
    public string? CpusetCpus { get; set; }
    public string? CpusetMems { get; set; }
    public List<object>? Devices { get; set; }
    public object? DeviceCgroupRules { get; set; }
    public object? DeviceRequests { get; set; }
    public int KernelMemory { get; set; }
    public int KernelMemoryTCP { get; set; }
    public int MemoryReservation { get; set; }
    public int MemorySwap { get; set; }
    public object? MemorySwappiness { get; set; }
    public bool OomKillDisable { get; set; }
    public object? PidsLimit { get; set; }
    public object? Ulimits { get; set; }
    public int CpuCount { get; set; }
    public int CpuPercent { get; set; }
    public int IOMaximumIOps { get; set; }
    public int IOMaximumBandwidth { get; set; }
    public List<string>? MaskedPaths { get; set; }
    public List<string>? ReadonlyPaths { get; set; }
}

public class Data
{
    public string? LowerDir { get; set; }
    public string? MergedDir { get; set; }
    public string? UpperDir { get; set; }
    public string? WorkDir { get; set; }
}

public class GraphDriver
{
    public Data? Data { get; set; }
    public string? Name { get; set; }
}

public class Mount
{
    public string? Type { get; set; }
    public string? Name { get; set; }
    public string? Source { get; set; }
    public string? Destination { get; set; }
    public string? Driver { get; set; }
    public string? Mode { get; set; }
    public bool RW { get; set; }
    public string? Propagation { get; set; }
}

public class ExposedPorts
{
    [JsonProperty("5432/tcp")] public PortBinding? _PortBinding { get; set; }
}

public class VarLibPostgresqlData
{
}

public class Volumes
{
    [JsonProperty("/var/lib/postgresql/data")] public VarLibPostgresqlData? VarLibPostgresqlData { get; set; }
}

public class Labels
{
    public string? maintainer { get; set; }
}

public class Ports
{
    [JsonProperty("5432/tcp")] public List<PortBinding>? PortBinding { get; set; }
}

public class Bridge
{
    public object? IPAMConfig { get; set; }
    public object? Links { get; set; }
    public object? Aliases { get; set; }
    public string? NetworkID { get; set; }
    public string? EndpointID { get; set; }
    public string? Gateway { get; set; }
    public string? IPAddress { get; set; }
    public int IPPrefixLen { get; set; }
    public string? IPv6Gateway { get; set; }
    public string? GlobalIPv6Address { get; set; }
    public int GlobalIPv6PrefixLen { get; set; }
    public string? MacAddress { get; set; }
    public object? DriverOpts { get; set; }
}

public class Networks
{
    public Bridge? bridge { get; set; }
}

public class NetworkSettings
{
    public string? Bridge { get; set; }
    public string? SandboxID { get; set; }
    public bool HairpinMode { get; set; }
    public string? LinkLocalIPv6Address { get; set; }
    public int LinkLocalIPv6PrefixLen { get; set; }
    public Ports? Ports { get; set; }
    public string? SandboxKey { get; set; }
    public object? SecondaryIPAddresses { get; set; }
    public object? SecondaryIPv6Addresses { get; set; }
    public string? EndpointID { get; set; }
    public string? Gateway { get; set; }
    public string? GlobalIPv6Address { get; set; }
    public int GlobalIPv6PrefixLen { get; set; }
    public string? IPAddress { get; set; }
    public int IPPrefixLen { get; set; }
    public string? IPv6Gateway { get; set; }
    public string? MacAddress { get; set; }
    public Networks? Networks { get; set; }
}

public class InspectResult
{
    public string? Id { get; set; }
    public string? Created { get; set; }
    public string? Path { get; set; }
    public List<string>? Args { get; set; }
    public State? State { get; set; }
    public string? Image { get; set; }
    public string? ResolvConfPath { get; set; }
    public string? HostnamePath { get; set; }
    public string? HostsPath { get; set; }
    public string? LogPath { get; set; }
    public string? Name { get; set; }
    public int RestartCount { get; set; }
    public string? Driver { get; set; }
    public string? Platform { get; set; }
    public string? MountLabel { get; set; }
    public string? ProcessLabel { get; set; }
    public string? AppArmorProfile { get; set; }
    public object? ExecIDs { get; set; }
}