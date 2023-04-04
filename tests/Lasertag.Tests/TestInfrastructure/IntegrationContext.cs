using Alba;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using MQTTnet.Client;
using Wolverine.Tracking;
using Xunit;

namespace Lasertag.Tests.TestInfrastructure;

public abstract class IntegrationContext : IAsyncLifetime
{
    protected IntegrationContext(AppFixture fixture)
    {
        Host = fixture.Host!;
        Store = Host.Services.GetRequiredService<IDocumentStore>();
        MqttClient = Host.Services.GetRequiredService<IMqttClient>();
    }

    public IAlbaHost Host { get; }
    public IDocumentStore Store { get; }
    public IMqttClient MqttClient { get; }

    public async Task InitializeAsync()
    {
        // Using Marten, wipe out all data and reset the state
        // back to exactly what we described in InitialAccountData
        await Store.Advanced.ResetAllData();
        await ConnectMqttClient();
    }

    // This is required because of the IAsyncLifetime 
    // interface. Note that I do *not* tear down database
    // state after the test. That's purposeful
    public Task DisposeAsync() =>
        Task.CompletedTask;

    async Task ConnectMqttClient()
    {
        var options = Host.Services.GetRequiredService<MqttClientOptions>();
        await MqttClient.ConnectAsync(options);
    }

    protected Task<(ITrackedSession, IScenarioResult?)> TrackedHttpCall(Action<Scenario> configuration) =>
        TrackedHttpCall(configuration, TimeSpan.FromSeconds(5));

    // This method allows us to make HTTP calls into our system
    // in memory with Alba, but do so within Wolverine's test support
    // for message tracking to both record outgoing messages and to ensure
    // that any cascaded work spawned by the initial command is completed
    // before passing control back to the calling test
    protected async Task<(ITrackedSession, IScenarioResult?)> TrackedHttpCall(Action<Scenario> configuration,
        TimeSpan timeout)
    {
        IScenarioResult? result = null;

        // The outer part is tying into Wolverine's test support
        // to "wait" for all detected message activity to complete
        var tracked = await Host.ExecuteAndWaitAsync(async () =>
            {
                // The inner part here is actually making an HTTP request
                // to the system under test with Alba
                result = await Host.Scenario(configuration);
            },
            (int)timeout.TotalMilliseconds);

        return (tracked, result);
    }
}