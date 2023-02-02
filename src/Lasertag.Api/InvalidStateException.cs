namespace Lasertag.Api;

#pragma warning disable S3925
[Serializable]
public class InvalidStateException : Exception
{
    public InvalidStateException(string message) : base(message)
    {
    }
}
#pragma warning restore S3925