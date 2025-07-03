using Hangfire;
using Hangfire.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ProjectPet.FileService.Infrastructure.MongoDb;
using ProjectPet.FileService.Options;
using System.Diagnostics.CodeAnalysis;

namespace ProjectPet.FileService.Infrastructure.Hangfire;

public static class HangfireExtensions
{
    public static IHostApplicationBuilder AddHangfire(this IHostApplicationBuilder builder)
    {
        builder.Services
            .AddHangfire((sp, configuration) =>
            {
                var mongoOptions = sp.GetRequiredService<IOptions<MongoDbOptions>>().Value;

                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseMongoStorage(
                        sp.GetRequiredService<IMongoClient>(),
                        mongoOptions!.JobDBName,
                        MongoStorageOptionsFactory.Create()
                    );
            }
            );

        // Add the processing server as IHostedService
        builder.Services.AddHangfireServer(opts => opts.ServerName = "Hangfire.Mongo server 1");

        return builder;
    }

    public static IApplicationBuilder UseAuthlessHangfireDashboard(
        [NotNull] this IApplicationBuilder app,
        [NotNull] string pathMatch = "/hangfire")
    {
        app.UseHangfireDashboard(
            pathMatch,
            new DashboardOptions()
            {
                Authorization = new[] { new HangFireDisabledAuthorizationFilter() },
            }
        );
        return app;
    }
}
