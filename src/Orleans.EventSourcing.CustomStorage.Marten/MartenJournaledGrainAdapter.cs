﻿using Marten;

namespace Orleans.EventSourcing.CustomStorage.Marten;

public class MartenJournaledGrainAdapter<TState, TEvent>
    where TState : class, new()
    where TEvent : class
{
    readonly IDocumentStore _store;

    public MartenJournaledGrainAdapter(IDocumentStore store)
    {
        _store = store;
    }

    public async Task<KeyValuePair<int, TState>> ReadStateFromStorage(Guid id)
    {
        var session = _store.LightweightSession();
        await using var _ = session.ConfigureAwait(false);

        var stream = await session.Events.FetchStreamAsync(id).ConfigureAwait(false);

        var state = default(TState);
        if (stream.Any())
        {
            state = await session.Events.AggregateStreamAsync<TState>(id).ConfigureAwait(false);
        }

        state ??= new TState();
        return new KeyValuePair<int, TState>(stream.Count, state);
    }

    public async Task<bool> ApplyUpdatesToStorage(Guid id, IReadOnlyList<TEvent> updates, int expectedversion)
    {
        var session = _store.OpenSession();
        await using var _ = session.ConfigureAwait(false);

        var stream = await session.Events.FetchStreamAsync(id).ConfigureAwait(false);
        if (stream.Count != expectedversion)
        {
            return false;
        }

        if (stream.Count == 0)
        {
            session.Events.StartStream<TState>(id, updates);
        }
        else
        {
            await session.Events.AppendExclusive(id, updates.Cast<object>().ToArray()).ConfigureAwait(false);
        }

        await session.SaveChangesAsync().ConfigureAwait(false);
        return true;
    }
}