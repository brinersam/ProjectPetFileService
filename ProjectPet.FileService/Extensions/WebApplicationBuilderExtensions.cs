using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;
using ProjectPet.FileService.Infrastructure.Providers;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddS3(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<S3Options>(
            builder.Configuration.GetRequiredSection(S3Options.SECTION));

        builder.Services.AddSingleton<IAmazonS3>(services =>
        {
            var options = services.GetService<IOptions<S3Options>>()!.Value;

            var creds = new BasicAWSCredentials(options.AccessKey, options.SecretKey);

            var config = new AmazonS3Config
            {
                ServiceURL = options.Endpoint,
                UseHttp = !options.WithSsl,
            };

            return new AmazonS3Client(creds, config);
        });

        builder.Services.AddSingleton<IS3Provider, S3Provider>();

        return builder;
    }
}
