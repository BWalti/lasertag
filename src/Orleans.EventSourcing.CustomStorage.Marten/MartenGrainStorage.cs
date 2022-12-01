using Marten;
using Orleans.Runtime;
using Orleans.Storage;

namespace Orleans.EventSourcing.CustomStorage.Marten;

public class MartenGrainStorage : IGrainStorage
{
    readonly IDocumentSession _session;

    public MartenGrainStorage(IDocumentSession session)
    {
        _session = session;
    }


    public async Task ReadStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var hashCode = grainId.GetUniformHashCode();
        _ = Guid.TryParse(grainState.ETag, out var etagAsGuid);

        var existing = await _session.Query<MartenGrainWrapper<T>>()
            .SingleOrDefaultAsync(item => item.GrainType == stateName && item.HashCode == hashCode)
            .ConfigureAwait(false);

        if (existing != null)
        {
            grainState.RecordExists = true;
            grainState.ETag = existing.Version.ToString();
            grainState.State = existing.Payload!;
        }
        else
        {
            grainState.RecordExists = false;
        }
    }

    public async Task WriteStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var hashCode = grainId.GetUniformHashCode();
        _ = Guid.TryParse(grainState.ETag, out var etagAsGuid);

        if (grainState.RecordExists)
        {
            var existing = await _session.Query<MartenGrainWrapper<T>>()
                .SingleAsync(item => item.GrainType == stateName && item.HashCode == hashCode).ConfigureAwait(false);

            existing.Payload = grainState.State;
            existing.Version = etagAsGuid;

            _session.Store(existing);
        }
        else
        {
            var item = new MartenGrainWrapper<T>
            {
                Id = Guid.NewGuid(),
                GrainType = stateName,
                HashCode = hashCode,
                Payload = grainState.State
            };

            _session.Store(item);
        }

        await _session.SaveChangesAsync().ConfigureAwait(false);
    }


    public async Task ClearStateAsync<T>(string stateName, GrainId grainId, IGrainState<T> grainState)
    {
        var hashCode = grainId.GetUniformHashCode();
        _ = Guid.TryParse(grainState.ETag, out var etagAsGuid);

        if (grainState.RecordExists)
        {
            var existing = await _session.Query<MartenGrainWrapper<T>>()
                .SingleAsync(item => item.GrainType == stateName && item.HashCode == hashCode).ConfigureAwait(false);

            _session.Delete(existing);
        }

        await _session.SaveChangesAsync().ConfigureAwait(false);
    }
}