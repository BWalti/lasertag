namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record DomainEventBase : IDomainEventBase
{
    public static readonly int NewEtag = -1;

    [Id(0)] public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

    [Id(1)] public int ETag { get; set; } = NewEtag;
}