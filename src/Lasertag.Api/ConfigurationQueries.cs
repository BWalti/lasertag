using Lasertag.DomainModel;
using Lasertag.Manager.Configuration;
using Orleans;

namespace Lasertag.Api;

public class ConfigurationQueries : Grain, IConfigurationQueries
{
    public async Task<ApiResult<Configuration>> GetCurrentConfiguration(Guid id)
    {
        var configurationManager = GrainFactory.GetGrain<IConfigurationManager>(id);
        try
        {
            var configuration = await configurationManager.GetManagedState();
            return new ApiResult<Configuration>(configuration);
        }
        catch (Exception e)
        {
            return new ApiResult<Configuration>(e);
        }
    }
}