using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Infrastructure.Caching;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Infrastructure.AmazonS3;

public class S3ProviderCachingDecorator : IS3Provider
{
    private readonly S3Provider _provider;
    private readonly ICachingService _caching;

    public S3ProviderCachingDecorator(
        S3Provider provider,
        ICachingService caching)
    {
        _provider = provider;
        _caching = caching;
    }

    public async Task<Result<FileUrlDto, Error>> CreatePresignedDownloadUrlAsync(FileLocationDto location, int expirationHours)
    {
        var key = CachingKeys.GetKey(location);
        var cached = await _caching.GetAsync<FileUrlDto>(key);
        if (cached.IsSuccess)
            return cached.Value;

        var result = await _provider.CreatePresignedDownloadUrlAsync(location, expirationHours);
        if (result.IsFailure)
            return result.Error;

        key = CachingKeys.GetKey(location);
        await _caching.SetAsync(key, result.Value);
        return result;
    }

    public async Task<Result<string, Error>> CreatePresignedUploadUrlAsync(FileLocationDto location, string uploadId, int partNumber)
    {
        return await _provider.CreatePresignedUploadUrlAsync(location, uploadId, partNumber);
    }

    public async Task<Result<string, Error>> CreatePresignedUploadUrlAsync(string fileName, FileLocationDto location)
    {
        return await _provider.CreatePresignedUploadUrlAsync(fileName, location);
    }

    public async Task<Result<string, Error>> DeleteFileAsync(FileLocationDto location, CancellationToken ct)
    {
        var key = CachingKeys.GetKey(location);

        await _caching.RemoveAsync(key, ct);

        return await _provider.DeleteFileAsync(location, ct);
    }

    public async Task<Result<List<string>, Error>> ListBucketsAsync(CancellationToken ct)
    {
        return await _provider.ListBucketsAsync(ct);
    }

    public async Task<Result<List<MultipartUpload>, Error>> ListMultipartUploadsAsync(string bucketName, CancellationToken ct)
    {
        return await _provider.ListMultipartUploadsAsync(bucketName, ct);
    }

    public async Task<UnitResult<Error>> MultipartUploadAbortAsync(FileLocationDto location, string uploadId, CancellationToken ct)
    {
        return await _provider.MultipartUploadAbortAsync(location, uploadId, ct);
    }

    public async Task<Result<FileLocationDto, Error>> MultipartUploadCompleteAsync(FileLocationDto location, string uploadId, IEnumerable<PartETagDto> partETags, CancellationToken ct)
    {
        return await _provider.MultipartUploadCompleteAsync(location, uploadId, partETags, ct);
    }

    public async Task<Result<string, Error>> MultipartUploadStartAsync(string fileName, string contentType, FileLocationDto location, CancellationToken ct)
    {
        return await _provider.MultipartUploadStartAsync(fileName, contentType, location, ct);
    }

    public async Task<UnitResult<Error>> UploadFileAsync(FileLocationDto location, string? contentType, Stream file, CancellationToken ct)
    {
        return await _provider.UploadFileAsync(location, contentType, file, ct);
    }
}
