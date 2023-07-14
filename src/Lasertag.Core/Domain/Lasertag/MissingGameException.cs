namespace Lasertag.Core.Domain.Lasertag;

#pragma warning disable S3925
public class MissingGameException : Exception
#pragma warning restore S3925
{
    public MissingGameException(Guid gameId) : base($"Game with id {gameId} is unknown!")
    {
    }
}