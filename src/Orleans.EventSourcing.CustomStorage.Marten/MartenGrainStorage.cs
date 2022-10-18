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

    public async Task ReadStateAsync<T>(string grainType, GrainReference grainReference, IGrainState<T> grainState)
    {
        var hashCode = grainReference.GetUniformHashCode();
        _ = Guid.TryParse(grainState.ETag, out var etagAsGuid);

        var existing = await _session.Query<MartenGrainWrapper<T>>()
            .SingleOrDefaultAsync(item => item.GrainType == grainType && item.HashCode == hashCode);

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

    public async Task WriteStateAsync<T>(string grainType, GrainReference grainReference, IGrainState<T> grainState)
    {
        var hashCode = grainReference.GetUniformHashCode();
        _ = Guid.TryParse(grainState.ETag, out var etagAsGuid);

        if (grainState.RecordExists)
        {
            var existing = await _session.Query<MartenGrainWrapper<T>>()
                .SingleAsync(item => item.GrainType == grainType && item.HashCode == hashCode);

            existing.Payload = grainState.State;
            existing.Version = etagAsGuid;

            _session.Store(existing);

        }
        else
        {
            var item = new MartenGrainWrapper<T>
            {
                Id = Guid.NewGuid(),
                GrainType = grainType,
                HashCode = hashCode,
                Payload = grainState.State
            };

            _session.Store(item);
        }
        await _session.SaveChangesAsync();
    }

    public async Task ClearStateAsync<T>(string grainType, GrainReference grainReference, IGrainState<T> grainState)
    {
        var hashCode = grainReference.GetUniformHashCode();
        _ = Guid.TryParse(grainState.ETag, out var etagAsGuid);

        if (grainState.RecordExists)
        {
            var existing = await _session.Query<MartenGrainWrapper<T>>()
                .SingleAsync(item => item.GrainType == grainType && item.HashCode == hashCode);

            _session.Delete(existing);
        }

        await _session.SaveChangesAsync();
    }
}