using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameEventBase : IGameEventBase
{
    public static readonly int NewEtag = -1;

    [Id(0)]
    public DateTimeOffset Timestamp = DateTimeOffset.Now;

    [Id(1)]
    public int ETag { get; set; } = NewEtag;
}