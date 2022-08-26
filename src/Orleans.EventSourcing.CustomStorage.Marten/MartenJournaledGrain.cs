using Marten;

namespace Orleans.EventSourcing.CustomStorage.Marten;

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