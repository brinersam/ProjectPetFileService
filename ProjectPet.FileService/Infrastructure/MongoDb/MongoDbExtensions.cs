using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Infrastructure.MongoDb;

public static class MongoDbExtensions
{
    public static IHostApplicationBuilder AddMongoDb(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<MongoDbOptions>(
            builder.Configuration.GetSection(MongoDbOptions.SECTION));

        builder.Services.AddSingleton<IMongoClient, MongoClient>(sp =>
        {
            var mongoOptions = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;

            var mongoUrlBuilder = new MongoUrlBuilder(mongoOptions.Endpoint)
            {
                DatabaseName = mongoOptions.JobDBName,
            };
            return new MongoClient(mongoUrlBuilder.ToMongoUrl());
        }
        );
        return builder;
    }
}
