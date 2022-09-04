namespace Lasertag.DomainModel.DomainEvents;

public interface ITargetPlayerEvents
{
    public Guid TargetPlayerId { get; }
    public Guid TargetGroupId { get; }
    public Guid TargetGameSetId { get; }
}