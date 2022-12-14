namespace Orleans.EventSourcing.CustomStorage.Marten;

public class EventSourcedGrain<TDomainModel, TDomainState, TEventBase>
    : JournaledGrain<TDomainState, TEventBase>
        , IEventSourcedGrain<TDomainModel, TEventBase>
    where TDomainModel : class
    where TDomainState : class, TDomainModel, new()
    where TEventBase : class
{
    public Task<TDomainModel> GetManagedState() =>
        Task.FromResult((TDomainModel)State);

    public new Task RaiseEvent<TEvent>(TEvent @event)
        where TEvent : TEventBase
    {
        base.RaiseEvent(@event);
        return Task.CompletedTask;
    }

    public new Task RaiseEvents<TEvent>(IEnumerable<TEvent> events)
        where TEvent : TEventBase
    {
        base.RaiseEvents(events);
        return Task.CompletedTask;
    }

    public new Task<bool> RaiseConditionalEvent<TEvent>(TEvent @event)
        where TEvent : TEventBase =>
        base.RaiseConditionalEvent(@event);

    public new Task<bool> RaiseConditionalEvents<TEvent>(IEnumerable<TEvent> events)
        where TEvent : TEventBase =>
        base.RaiseConditionalEvents(events);

    public new async Task ConfirmEvents()
    {
        await base.ConfirmEvents();
    }
}