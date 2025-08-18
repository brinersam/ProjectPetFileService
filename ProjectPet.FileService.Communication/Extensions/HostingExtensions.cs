using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectPet.FileService.Contracts;

namespace ProjectPet.FileService.Communication.Extensions;
public static class HostingExtensions
{
    public static IHostApplicationBuilder AddFileServiceHttpClient(this IHostApplicationBuilder builder)
    {
        builder.Services.Configure<FileServiceOptions>(builder.Configuration.GetSection(FileServiceOptions.REGION));
        builder.Services.AddHttpClient<IFileService, FileServiceClient>();
        return builder;
    }
}
