using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameLobbyCreated(GameSetGroup[] GameSetGroups) : GameEventBase;