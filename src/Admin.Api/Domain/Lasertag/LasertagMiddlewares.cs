using Admin.Api.Domain.Account;
using Admin.Api.Extensions;
using Marten;
using Wolverine;

namespace Admin.Api.Domain.Lasertag;
// ReSharper disable UnusedMember.Global
#pragma warning disable S1118

public class ServerLookupMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<(HandlerContinuation, Server?)> LoadAsync(
        LasertagCommands.IServerCommands command,
        ILogger<ServerLookupMiddleware> logger,
        // This app is using Marten for persistence
        IDocumentSession session,
        CancellationToken cancellation)
    {
        using var activity = OpenTelemetryExtensions.ActivitySource.StartActivity(nameof(ServerLookupMiddleware));

        var server = await session.LoadAsync<Server>(command.ServerId, cancellation);
        if (server == null)
        {
            logger.LogWarning("Unable to find a Server for {ServerId}, aborting the requested operation",
                command.ServerId);
        }
        else
        {
            logger.LogInformation("Loaded Server for {ServerId}", command.ServerId);
        }

        return (server == null ? HandlerContinuation.Stop : HandlerContinuation.Continue, server);
    }
}


#pragma warning restore S1118