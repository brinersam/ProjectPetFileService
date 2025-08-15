using MassTransit;
using Microsoft.Extensions.Options;
using ProjectPet.Core.Options;

namespace ProjectPet.FileService.Infrastructure.RabbitMQ;

public static class RabbitMQExtensions
{
    public static IHostApplicationBuilder AddRabbitMQ(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMQOptions>(builder.Configuration.GetSection(RabbitMQOptions.REGION));
        builder.Services.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();

            config.AddConsumers(typeof(RabbitMQExtensions).Assembly);

            config.UsingRabbitMq((context, config) =>
            {
                var options = context.GetRequiredService<IOptions<RabbitMQOptions>>().Value;
                config.Host(
                    new Uri(options.Host),
                    configure =>
                    {
                        configure.Username(options.Username);
                        configure.Password(options.Password);
                    }
                );

                config.Durable = true;

                config.ConfigureEndpoints(context);
            });
        });
        return builder;

    }
}
