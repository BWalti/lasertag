using Orleans;
using Orleans.EventSourcing.CustomStorage;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Lasertag.Manager;

public interface IDomainManager<TDomainModel, TState, TDomainEvent> :
    IGrainWithGuidKey,
    IEventSourcedGrain<TDomainModel, TDomainEvent>,
    ICustomStorageInterface<TState, TDomainEvent>
    where TDomainModel : class
    where TDomainEvent : class
{
}