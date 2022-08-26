using Orleans;

namespace GrainInterfaces.Models;

[GenerateSerializer]
public record GameGroup(Guid Id, Guid[] GameSetIds);