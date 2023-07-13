using Alba;
using Microsoft.AspNetCore.Hosting;
using Oakton;
using Wolverine;
using Xunit;

namespace Lasertag.Tests.TestInfrastructure;

public class AppFixture : IAsyncLifetime
{
    public IAlbaHost? Host { get; private set; }

    public async Task InitializeAsync()
    {
        // Workaround for Oakton with WebApplicationBuilder
        // lifecycle issues. Doesn't matter to you w/o Oakton
        OaktonEnvironment.AutoStartHost = true;

#pragma warning disable S3220
        // This is bootstrapping the actual application using
        // its implied Program.Main() set up,
        // overriding some service configuration
        Host = await AlbaHost.For<Program>(ConfigureWebHostBuilder);
#pragma warning restore S3220
    }

    void ConfigureWebHostBuilder(IWebHostBuilder webHostBuilder)
    {
        // I'm overriding 
        webHostBuilder.ConfigureServices(services =>
        {
            // Let's just take any pesky message brokers out of
            // our integration tests for now so we can work in
            // isolation
            services.DisableAllExternalWolverineTransports();

            // Just putting in some baseline data for our database
            // There's usually *some* sort of reference data in 
            // enterprise-y systems
            //services.InitializeMartenWith<InitialAccountData>()
        });
    }

    public Task DisposeAsync() =>
        Host == null ? Task.CompletedTask : Host.DisposeAsync().AsTask();
}