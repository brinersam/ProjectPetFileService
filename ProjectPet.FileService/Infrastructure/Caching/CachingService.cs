using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using ProjectPet.FileService.Options;
using ProjectPet.SharedKernel.ErrorClasses;
using System.Text.Json;

namespace ProjectPet.FileService.Infrastructure.Caching;

public class CachingService : ICachingService
{
    private readonly S3Options _options;
    private readonly IDistributedCache _cache;

    public CachingService(
        IOptions<S3Options> options,
        IDistributedCache cache)
    {
        _options = options.Value;
        _cache = cache;
    }

    public async Task<Result<T, Error>> GetAsync<T>(string key, CancellationToken cancellationToken = default) 
        where T : class
    {
        var json = await _cache.GetStringAsync(key, cancellationToken);
        if (json == null)
            return Errors.General.NotFound(typeof(T));

        var result = (T)JsonSerializer.Deserialize(json, typeof(T))!;
        return result;
    }

    public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default)
        where T : class
    {
        var cacheOptions = new DistributedCacheEntryOptions()
                           { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_options.UrlExpirationDays) };

        var json = JsonSerializer.Serialize(value, typeof(T));
        await _cache.SetStringAsync(key, json, cacheOptions, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _cache.RemoveAsync(key, cancellationToken);
    }
}