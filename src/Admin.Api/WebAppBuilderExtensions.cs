using System.Text.Json.Serialization;
using EasyNetQ.ConnectionString;
using Microsoft.AspNetCore.Http.Json;
using Orleans.Hosting;
using Orleans;

namespace Admin.Api;

public static class WebAppBuilderExtensions
{
    /// <summary>
    /// Use defaults for certain infrastructural things like Logging and <see cref="JsonOptions"/>.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure.</param>
    /// <returns>The configured <see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder UseDefaultInfrastructure(this WebApplicationBuilder builder)
    {
        builder.Logging
            .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
            .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
            .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
            .AddDebug() // VS Debug window
            .AddConsole();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors();

        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return builder;
    }

    /// <summary>
    /// Swagger generation tries to run the application, therefore we skip some parts of the "startup".
    /// Namely the Orleans ClusterClient and Queueing connectivity.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to configure.</param>
    /// <param name="args">The command line arguments - if includes "start", the actual connectivity will be built up.</param>
    /// <returns>The configured <see cref="WebApplicationBuilder"/></returns>
    public static WebApplicationBuilder UseSwaggerGeneratorHack(this WebApplicationBuilder builder, string[] args)
    {
        if (args.Contains("start", StringComparer.CurrentCulture))
        {
            builder.Services.AddOrleansClient(clientBuilder => { clientBuilder.UseLocalhostClustering(); });
            builder.Services.RegisterEasyNetQ(resolver =>
            {
                var parser = resolver.Resolve<IConnectionStringParser>();
                var configuration = resolver.Resolve<IConfiguration>();

                var connectionString = configuration["Mq:Host"];
                return parser.Parse(connectionString);
            });
        }
        else
        {
            builder.Services.AddSingleton<IClusterClient>(provider => new MockClusterClient(provider));
        }

        return builder;
    }
}