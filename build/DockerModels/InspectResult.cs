﻿namespace Lasertag.Builder.DockerModels;

#pragma warning disable MA0016
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