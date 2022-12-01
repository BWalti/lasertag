using Orleans.Runtime;

namespace Lasertag.Manager.Server;

public class ServerGrain : Grain, IServerGrain
{
    readonly IPersistentState<DomainModel.Server> _serverState;

    public ServerGrain([PersistentState(nameof(ServerGrain))] IPersistentState<DomainModel.Server> serverState)
    {
        _serverState = serverState;
    }

    public async Task DoIt()
    {
        _serverState.State.Name = "Hello World";
        await _serverState.WriteStateAsync().ConfigureAwait(false);
    }
}