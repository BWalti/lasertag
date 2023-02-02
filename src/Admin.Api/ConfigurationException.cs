using System.Runtime.Serialization;

namespace Admin.Api;

#pragma warning disable S3925
#pragma warning disable CS0628

[Serializable]
public sealed class ConfigurationException : Exception
{
    public ConfigurationException()
    {
    }

    public ConfigurationException(string message) : base(message)
    {
    }

    public ConfigurationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Without this constructor, deserialization will fail
    protected ConfigurationException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

}

#pragma warning restore CS0628
#pragma warning restore S3925
