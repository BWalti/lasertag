﻿// See https://aka.ms/new-console-template for more information

using Lasertag.IoT.Simulator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Client;
using OpenTelemetry.Exporter;

var host = Host.CreateDefaultBuilder()
    .UseConsoleLifetime()
    .ConfigureLogging(builder =>
    {
        builder
            .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
            .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
            .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
            .AddDebug() // VS Debug window
            .AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddOptions();
        services.Configure<OtlpExporterOptions>(context.Configuration.GetSection("OtlpExporter"));
        services.Configure<SimulatorOptions>(context.Configuration.GetSection("Simulator"));

        services.AddHttpClient<IotInfrastructureCalls>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<SimulatorOptions>>();
            client.BaseAddress = options.Value.ApiBaseAddress;
        });

        services.AddOpenTelemetry()
            .WithMetrics()
            .WithTracing();

        services.AddSingleton<MqttFactory>();
        services.AddTransient(provider => provider.GetRequiredService<MqttFactory>().CreateMqttClient());
        services.AddTransient(_ =>
        {
            var section = context.Configuration.GetSection("Mqtt");
            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(builder => builder.WithUri(section["Server"]))
                .Build();

            return options;
        });

        services.AddSingleton<IMqttAdapter, MqttAdapter>();
        services.AddSingleton<IotStateMachine>();
        services.AddHostedService<IotSimulatorHostedService>();
    })
    .Build();

host.Run();