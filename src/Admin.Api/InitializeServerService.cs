using Admin.Api.Contexts;
using Marten;
using Wolverine;

namespace Admin.Api;

public class InitializeServerService : IHostedService
{
    readonly IDocumentStore _store;
    readonly IMessageBus _bus;

    public InitializeServerService(IDocumentStore store, IMessageBus bus)
    {
        _store = store;
        _bus = bus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var session = _store.LightweightSession();
        var server = session.Load<Server>(Guid.Empty);

        if (server == null)
        {
            server = new Server();
            session.Store(server);

            await session.SaveChangesAsync(cancellationToken);
        }

        await _bus.PublishAsync(new ServerStarted());
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
