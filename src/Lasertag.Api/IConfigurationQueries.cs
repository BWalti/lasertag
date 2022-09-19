using Lasertag.DomainModel;
using Orleans;

namespace Lasertag.Api;

public interface IConfigurationQueries : IGrainWithIntegerKey
{
    public Task<ApiResult<Configuration>> GetCurrentConfiguration(Guid id);
}