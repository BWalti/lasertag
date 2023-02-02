using System.Text.Json.Serialization;
using EasyNetQ.ConnectionString;
using Microsoft.AspNetCore.Http.Json;

namespace Admin.Api;

public static class WebAppBuilderExtensions
{
    /// <summary>
    ///     Use defaults for certain infrastructural things like Logging and <see cref="JsonOptions" />.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> to configure.</param>
    /// <returns>The configured <see cref="WebApplicationBuilder" /></returns>
    public static WebApplicationBuilder UseDefaultInfrastructure(this WebApplicationBuilder builder)
    {


        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors();

        builder.Services.Configure<JsonOptions>(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o =>
            o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        return builder;
    }

    /// <summary>
    ///     Swagger generation tries to run the application, therefore we skip some parts of the "startup".
    ///     Namely the Orleans ClusterClient and Queueing connectivity.
    /// </summary> 
    /// <param name="builder">The <see cref="WebApplicationBuilder" /> to configure.</param>
    /// <param name="args">The command line arguments - if includes "start", the actual connectivity will be built up.</param>
    /// <returns>The configured <see cref="WebApplicationBuilder" /></returns>
    public static WebApplicationBuilder UseSwaggerGeneratorHack(this WebApplicationBuilder builder, string[] args)
    {
        builder.Services.RegisterEasyNetQ(resolver =>
        {
            var parser = resolver.Resolve<IConnectionStringParser>();
            var configuration = resolver.Resolve<IConfiguration>();

            var connectionString = configuration["Mq:Host"];
            return parser.Parse(connectionString);
        });

        return builder;
    }
}