using Admin.Api;
using Admin.Api.Domain.Account;
using Admin.Api.Domain.Lasertag;
using Admin.Api.Extensions;
using JasperFx.Core;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Exceptions;
using Npgsql;
using Oakton;
using Oakton.Resources;
using Weasel.Core;
using Wolverine;
using Wolverine.ErrorHandling;
using Wolverine.Marten;
using Wolverine.RabbitMQ;

#pragma warning disable S125

var builder = WebApplication
    .CreateBuilder(args)
    .UseDefaultInfrastructure()
    .AddOpenTelemetry();

builder.Host
    .ApplyOaktonExtensions()
    .UseWolverine(opts =>
    {
        var rabbitSettings = new RabbitMqSettings();
        builder.Configuration.GetSection("RabbitMq").Bind(rabbitSettings);
        opts.UseRabbitMq(rabbit =>
            {
                rabbit.HostName = rabbitSettings.HostName;
                rabbit.UserName = rabbitSettings.UserName;
                rabbit.Password = rabbitSettings.Password;
            })
            // Just do the routing off of conventions, more or less
            // queue and/or exchange based on the Wolverine message type name
            .UseConventionalRouting()
            // build any declared queues, exchanges, or bindings with the
            // Rabbit MQ broker as part of bootstrapping time
            .AutoProvision()
            .ConfigureSenders(x => x.UseDurableOutbox());

        //// I'm choosing to process any ChartingFinished event messages
        //// in a separate, local queue with persistent messages for the inbox/outbox
        //opts.PublishMessage<ChartingFinished>()
        //    .ToLocalQueue("charting")
        //    .UseDurableInbox();

        opts.Handlers.OnException<ConcurrencyException>().RetryTimes(3);
        opts.Handlers.OnException<NpgsqlException>()
            .RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());

        opts.Handlers.AddMiddlewareByMessageType(typeof(AccountLookupMiddleware));
        opts.Handlers.AddMiddlewareByMessageType(typeof(ServerLookupMiddleware));
    });

builder.Services.AddResourceSetupOnStartup();

builder.Services.AddMqttClient(builder.Configuration.GetSection("Mqtt"));
builder.Services.AddHostedService<MqttAdapterService>();

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

        //opts.Projections.Add<AppointmentDurationProjection>(ProjectionLifecycle.Async)
    })
    .AddAsyncDaemon(DaemonMode.HotCold)
    .IntegrateWithWolverine()
    .ApplyAllDatabaseChangesOnStartup()
    .EventForwardingToWolverine();

var app = builder.Build();

app.UseDevelopmentDefaults();

app.MapGroup("/api/lasertag")
    .MapLasertagApi()
    .WithTags("Lasertag");

app.MapGroup("/api/accounts")
    .MapAccountApi()
    .WithTags("Account");

return await app.RunOaktonCommands(args);