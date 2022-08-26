using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record GameSetGroup(Guid GroupId, GameSet[] GameSets, GroupColor Color)
{
    
}