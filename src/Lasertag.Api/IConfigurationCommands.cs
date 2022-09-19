using Lasertag.DomainModel;
using Orleans;

namespace Lasertag.Api;

public interface IConfigurationCommands : IGrainWithIntegerKey
{
    Task<ApiResult<Configuration>> RegisterLasertagSet(Guid id, Guid lasertagSetId);
}