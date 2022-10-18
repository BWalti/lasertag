using Orleans;

namespace Lasertag.Manager.Server;

public interface IServerGrain : IGrainWithIntegerKey
{
    public Task DoIt();
}