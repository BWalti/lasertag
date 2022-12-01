﻿using Newtonsoft.Json;

namespace Lasertag.Builder.DockerModels;

#pragma warning disable MA0016
public class Ports
{
    [JsonProperty("5432/tcp")] public List<PortBinding>? PortBinding { get; set; }
}