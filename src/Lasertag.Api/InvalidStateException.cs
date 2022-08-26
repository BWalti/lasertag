using Orleans;

namespace Lasertag.Api;

[GenerateSerializer]
public class InvalidStateException : Exception
{
    public InvalidStateException(string message) : base(message)
    {
    }
}