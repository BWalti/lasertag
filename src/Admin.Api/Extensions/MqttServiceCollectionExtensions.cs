using System.Globalization;
using MQTTnet;
using MQTTnet.Client;

namespace Admin.Api.Extensions;

public static class MqttServiceCollectionExtensions
{
    public static IServiceCollection AddMqttClient(this IServiceCollection services,
        IConfigurationSection configurationSection)
    {
        services.AddSingleton<MqttFactory>();
        services.AddTransient(provider => provider.GetRequiredService<MqttFactory>().CreateMqttClient());
        services.AddTransient(_ =>
        {
            var keepAlivePeriod = configurationSection["KeepAlivePeriod"] ?? TimeSpan.FromSeconds(15).ToString();

            var options = new MqttClientOptionsBuilder()
                .WithWebSocketServer(builder =>
                {
                    builder.WithUri(configurationSection["Server"]);
                })
                .WithKeepAlivePeriod(TimeSpan.Parse(keepAlivePeriod, CultureInfo.InvariantCulture))
                .Build();

            return options;
        });

        return services;
    }
}