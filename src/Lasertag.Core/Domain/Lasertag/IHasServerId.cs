namespace Lasertag.Core.Domain.Lasertag;

public interface IHasServerId
{
    public Guid ServerId { get; }
}

public interface IHasGameId
{
    public Guid GameId { get; }
}