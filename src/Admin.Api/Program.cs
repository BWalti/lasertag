using Admin.Api;
using Admin.Api.Domain.Account;
using Admin.Api.Domain.Lasertag;
using Admin.Api.Extensions;
using JasperFx.Core;
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
using Wolverine.Marten;

#pragma warning disable S125

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

        opts.Projections.SelfAggregate<Game>();
        opts.Projections.Add<GameStatisticsProjection>(ProjectionLifecycle.Inline);
    })
    .AddAsyncDaemon(DaemonMode.HotCold)
    .ApplyAllDatabaseChangesOnStartup()
    .EventForwardingToWolverine(); // includes ".IntegrateWithWolverine()"

builder.Host
    .ApplyOaktonExtensions()
    .UseWolverine(opts =>
    {
        opts.Policies.AutoApplyTransactions();

        opts.LocalQueue("important")
            .UseDurableInbox();

        opts.OnException<ConcurrencyException>().RetryTimes(3);
        opts.OnException<NpgsqlException>()
            .RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());

        opts.Policies
            .ForMessagesOfType<AccountCommands.IAccountCommand>()
            .AddMiddleware<AccountLookupMiddleware>();

        opts.Policies
            .ForMessagesOfType<LasertagCommands.IServerCommands>()
            .AddMiddleware<ServerLookupMiddleware>();
    });

builder.Services.AddResourceSetupOnStartup();

builder.Services.AddMqttClient(builder.Configuration.GetSection("Mqtt"));
builder.Services.AddHostedService<MqttAdapterService>();

var app = builder.Build();

app.UseDevelopmentDefaults();

app.MapGroup("/api/lasertag")
    .MapLasertagApi()
    .WithTags("Lasertag");

app.MapGroup("/api/accounts")
    .MapAccountApi()
    .WithTags("Account");

return await app.RunOaktonCommands(args);