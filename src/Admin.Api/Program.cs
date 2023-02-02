using Admin.Api;
using Admin.Api.Contexts;
using Admin.Api.Extensions;
using JasperFx.Core;
using Lasertag.Messages;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Exceptions;
using MQTTnet;
using MQTTnet.Client;
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
    .UseSwaggerGeneratorHack(args);

builder.Host
    .UseWolverine(opts =>
    {
        opts.ListenToRabbitQueue("gamesets");

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
            // Directs Wolverine to build any declared queues, exchanges, or
            // bindings with the Rabbit MQ broker as part of bootstrapping time
            .AutoProvision()
            .ConfigureSenders(x => x.UseDurableOutbox());

        //// I'm choosing to process any ChartingFinished event messages
        //// in a separate, local queue with persistent messages for the inbox/outbox
        //opts.PublishMessage<ChartingFinished>()
        //    .ToLocalQueue("charting")
        //    .UseDurableInbox();

        // If we encounter a concurrency exception, just try it immediately
        // up to 3 times total
        opts.Handlers.OnException<ConcurrencyException>().RetryTimes(3);

        // It's an imperfect world, and sometimes transient connectivity errors
        // to the database happen
        opts.Handlers.OnException<NpgsqlException>()
            .RetryWithCooldown(50.Milliseconds(), 100.Milliseconds(), 250.Milliseconds());
    });

builder.Services.AddResourceSetupOnStartup();
builder.Services.AddHostedService<InitializeServerService>();

builder.Services.AddSingleton<MqttFactory>();
builder.Services.AddTransient(provider => provider.GetRequiredService<MqttFactory>().CreateMqttClient());
builder.Services.AddTransient(_ =>
{
    var section = builder.Configuration.GetSection("Mqtt");
    var options = new MqttClientOptionsBuilder()
        .WithWebSocketServer(section["Server"])
        .Build();

    return options;
});
builder.Services.AddHostedService<MqttAdapterService>();

builder.AddOpenTelemetry();
builder.Host.ApplyOaktonExtensions();

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

var app = builder
    .Build()
    .UseDevelopmentDefaults();

app.MapPost("/api/doSomething", (IMessageBus bus, ILogger<ClientConnected> logger) =>
{
    logger.LogError("Something is wrong!");
    return bus.PublishAsync(new ClientConnected(1));
});

app.MapGet("/api/server", (IQuerySession session) => session.Load<Server>(Guid.Empty));

return await app.RunOaktonCommands(args);