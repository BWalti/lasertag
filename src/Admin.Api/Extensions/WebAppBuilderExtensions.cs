using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;

namespace Admin.Api.Extensions;

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
}