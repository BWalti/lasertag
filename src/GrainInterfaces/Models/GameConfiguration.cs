using Orleans;

namespace GrainInterfaces.Models;

[GenerateSerializer]
public record GameConfiguration(int NumberOfGroups);