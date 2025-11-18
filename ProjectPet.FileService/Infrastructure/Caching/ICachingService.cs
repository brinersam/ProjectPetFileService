using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Infrastructure.Caching;

public interface ICachingService
{
    Task<Result<T, Error>> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class;

    Task SetAsync<T>(
        string key,
        T value,
        CancellationToken cancellationToken = default)
            where T : class;

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

}
