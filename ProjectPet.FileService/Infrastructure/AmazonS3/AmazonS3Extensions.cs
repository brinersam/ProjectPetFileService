using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Options;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Infrastructure.AmazonS3;

public static class AmazonS3Extensions
{
    public static WebApplicationBuilder AddAmazonS3(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<S3Options>(
            builder.Configuration.GetRequiredSection(S3Options.SECTION));

        builder.Services.AddSingleton<IAmazonS3>(services =>
        {
            var options = services.GetService<IOptions<S3Options>>()!.Value;

            var scheme = options.WithSsl ? "https" : "http";
            var endpoint = $"{scheme}://{options.Endpoint}";

            var creds = new BasicAWSCredentials(options.AccessKey, options.SecretKey);

            var config = new AmazonS3Config
            {
                ForcePathStyle = true,
                ServiceURL = endpoint,
                UseHttp = !options.WithSsl,
            };

            return new AmazonS3Client(creds, config);
        });

        builder.Services.AddSingleton<IS3Provider, S3Provider>();

        return builder;
    }
}
