using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectPet.FileService.Endpoints;

namespace ProjectPet.FileService.Extensions;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        return services.AddEndpoints(Assembly.GetExecutingAssembly());

    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, IEndpointRouteBuilder? routeBuilder = null)
    {
        var endpoints = app.Services.GetServices<IEndpoint>();

        routeBuilder ??= app;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(routeBuilder);
        }

        return app;
    }

    private static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndpoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndpoint), type));

        services.TryAddEnumerable(serviceDescriptors);
        return services;
    }
}
