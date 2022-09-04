using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameLobbyCreated(GameGroup[] GameSetGroups) : DomainEventBase;