
using ProjectPet.FileService.Infrastructure.Caching;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Infrastructure.Redis;

public static class RedisExtensions
{
    public static IHostApplicationBuilder AddRedis(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<OptionsRedis>(
            builder.Configuration.GetSection(OptionsRedis.SECTION));

        var redisOptions = builder.Configuration
            .GetSection(OptionsRedis.SECTION)
            .Get<OptionsRedis>();

        builder.Services.AddStackExchangeRedisCache(config =>
        {
            config.Configuration = redisOptions.Endpoint;
            config.InstanceName = "FileService";
        });

        builder.Services.AddSingleton<ICachingService, CachingService>();

        return builder;
    }

}
