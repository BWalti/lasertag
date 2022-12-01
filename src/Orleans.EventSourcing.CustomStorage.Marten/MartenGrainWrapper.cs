using Marten.Metadata;

namespace Orleans.EventSourcing.CustomStorage.Marten;

public class MartenGrainWrapper<T> : IVersioned
{
    public Guid Id { get; set; } = Guid.Empty;
    public string GrainType { get; set; } = string.Empty;
    public uint HashCode { get; set; }
    public T? Payload { get; set; }
    public Guid Version { get; set; }
}