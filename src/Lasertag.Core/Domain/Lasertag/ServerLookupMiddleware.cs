using Marten;
using Microsoft.Extensions.Logging;
using Wolverine;

namespace Lasertag.Core.Domain.Lasertag;
// ReSharper disable UnusedMember.Global
#pragma warning disable S1118

public class ServerLookupMiddleware
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<(HandlerContinuation, Server?)> LoadAsync(
        IHasServerId hasServerId,
        ILogger<ServerLookupMiddleware> logger,
        // This app is using Marten for persistence
        IDocumentSession session,
        CancellationToken cancellation)
    {
        using var activity = OpenTelemetryExtensions.ActivitySource.StartActivity(nameof(ServerLookupMiddleware));

        var server = await session.LoadAsync<Server>(hasServerId.ServerId, cancellation);
        if (server == null)
        {
            logger.LogWarning("Unable to find a Server for {ServerId}, aborting the requested operation",
                hasServerId.ServerId);
        }
        else
        {
            logger.LogInformation("Loaded Server for {ServerId}", hasServerId.ServerId);
        }

        return (server == null ? HandlerContinuation.Stop : HandlerContinuation.Continue, server);
    }
}

#pragma warning restore S1118