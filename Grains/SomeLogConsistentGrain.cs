using GrainInterfaces;
using Marten;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;

namespace Grains;

public class MartenJournaledGrain<TState, TEvent> : JournaledGrain<TState, TEvent>,
    ICustomStorageInterface<TState, TEvent>

    where TState : class, new()
    where TEvent : class
{
    private readonly IDocumentStore _store;

    public MartenJournaledGrain(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<KeyValuePair<int, TState>> ReadStateFromStorage()
    {
        await using var session = _store.LightweightSession();

        var stream = await session.Events.FetchStreamAsync(this.GetPrimaryKey());

        var state = default(TState);
        if (stream.Any())
            state = await session.Events.AggregateStreamAsync<TState>(this.GetPrimaryKey());

        state ??= new TState();
        return new KeyValuePair<int, TState>(stream.Count, state);
    }

    public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<TEvent> updates, int expectedversion)
    {
        await using var session = _store.OpenSession();

        var stream = await session.Events.FetchStreamAsync(this.GetPrimaryKey());
        if (stream.Count != expectedversion) return false;

        if (stream.Count == 0)
            session.Events.StartStream<TState>(this.GetPrimaryKey(), updates);
        else
            await session.Events.AppendExclusive(this.GetPrimaryKey(), updates.Cast<object>().ToArray());

        await session.SaveChangesAsync();
        return true;
    }
}

public class SomeMartenJournaledGrain :
    MartenJournaledGrain<SomeLogConsistentState, ISomeMartenLogEvents>,
    ISomeMartenJournaledGrain
{
    public SomeMartenJournaledGrain(IDocumentStore store) : base(store)
    {
    }

    public async Task RaiseAmount(int amount)
    {
        RaiseEvent(new SomeRaiseEvent
        {
            Amount = amount
        });

        await ConfirmEvents();
    }

    public async Task DecreaseAmount(int amount)
    {
        RaiseEvent(new SomeDecreaseEvent
        {
            Amount = amount
        });

        await ConfirmEvents();
    }

    public Task<int> GetAmount()
    {
        return Task.FromResult(State.Sum);
    }
}