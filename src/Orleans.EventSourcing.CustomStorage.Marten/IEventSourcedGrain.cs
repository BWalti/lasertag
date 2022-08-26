namespace Orleans.EventSourcing.CustomStorage.Marten;

public interface IEventSourcedGrain<TDomainModel, TEventBase>
    where TDomainModel : class
    where TEventBase : class
{
    Task<TDomainModel> GetManagedState();
    Task RaiseEvent<TEvent>(TEvent @event) where TEvent : TEventBase;
    Task RaiseEvents<TEvent>(IEnumerable<TEvent> events) where TEvent : TEventBase;
    Task<bool> RaiseConditionalEvent<TEvent>(TEvent @event) where TEvent : TEventBase;
    Task<bool> RaiseConditionalEvents<TEvent>(IEnumerable<TEvent> events) where TEvent : TEventBase;
    Task ConfirmEvents();
}