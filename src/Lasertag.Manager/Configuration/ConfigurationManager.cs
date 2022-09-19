using Lasertag.DomainModel.DomainEvents;
using Orleans;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Lasertag.Manager.Configuration;

public class ConfigurationManager : EventSourcedGrain<DomainModel.Configuration, ConfigurationState, IDomainEventBase>,
    IConfigurationManager
{
    readonly MartenJournaledGrainAdapter<ConfigurationState, IDomainEventBase> _martenAdapter;

    public ConfigurationManager(MartenJournaledGrainAdapter<ConfigurationState, IDomainEventBase> martenAdapter)
    {
        _martenAdapter = martenAdapter;
    }

    public Task<KeyValuePair<int, ConfigurationState>> ReadStateFromStorage() =>
        _martenAdapter.ReadStateFromStorage(this.GetPrimaryKey());

    public Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IDomainEventBase> updates, int expectedversion) =>
        _martenAdapter.ApplyUpdatesToStorage(this.GetPrimaryKey(), updates, expectedversion);
}