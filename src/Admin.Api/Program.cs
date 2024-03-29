using Admin.Api;
using Admin.Api.Extensions;
using JasperFx.Core;
using Lasertag.Core.Domain.Lasertag;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Marten.Exceptions;
using Npgsql;
using Oakton;
using Oakton.Resources;
using Weasel.Core;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Http;
using Wolverine.Marten;

var builder = WebApplication
    .CreateBuilder(args)
    .UseDefaultInfrastructure()
    .AddOpenTelemetry();

builder.Services.AddMarten(opts =>
    {
        var connectionString = builder.Configuration.GetConnectionString("Marten");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ConfigurationException("You have to configure the Marten connection string");
        }

        opts.Connection(connectionString);
        if (builder.Environment.IsDevelopment())
        {
            opts.AutoCreateSchemaObjects = AutoCreate.All;
        }

        opts.Projections.Snapshot<Server>(SnapshotLifecycle.Inline);
        opts.Projections.Snapshot<Game>(SnapshotLifecycle.Inline);
    })
    .UseLightweightSessions()
    .AddAsyncDaemon(DaemonMode.HotCold)
    .ApplyAllDatabaseChangesOnStartup()
    .EventForwardingToWolverine();

builder.Host
    .ApplyOaktonExtensions()
    .UseWolverine(opts =>
    {
        opts.Policies.AutoApplyTransactions();

        opts.LocalQueue("important").UseDurableInbox();

        opts.OnException<ConcurrencyException>().RetryTimes(3);
        opts.OnException<NpgsqlException>()
            .RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
    });

builder.Services.AddResourceSetupOnStartup();

builder.Services.AddMqttClient(builder.Configuration.GetSection("Mqtt"));
builder.Services.AddHostedService<MqttAdapterService>();

var app = builder.Build();
app.UseDevelopmentDefaults()
    .MapWolverineEndpoints();

return await app.RunOaktonCommands(args);