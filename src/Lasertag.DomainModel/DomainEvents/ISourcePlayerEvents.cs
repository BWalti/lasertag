namespace Lasertag.DomainModel.DomainEvents;

public interface ISourcePlayerEvents
{
    public Guid SourcePlayerId { get; }
    public Guid SourceGroupId { get; }
    public Guid SourceGameSetId { get; }
}