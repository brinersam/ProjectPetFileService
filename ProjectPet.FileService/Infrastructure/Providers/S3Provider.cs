using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using ProjectPet.FileService.Domain.FileManagment;
using ProjectPet.FileService.Options;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Infrastructure.Providers;

public class S3Provider : IS3Provider
{
    private readonly S3Options _options;
    private readonly IAmazonS3 _s3Client;
    private readonly ILogger<S3Provider> _logger;

    public S3Provider(
        IOptions<S3Options> options,
        IAmazonS3 s3Client,
        ILogger<S3Provider> logger)
    {
        _options = options.Value;
        _s3Client = s3Client;
        _logger = logger;
    }

    public async Task<Result<string, Error>> CreatePresignedUploadUrlAsync(
        FileLocation location,
        string uploadId,
        int partNumber)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                UploadId = uploadId,
                PartNumber = partNumber,
                Verb = HttpVerb.PUT,
                Protocol = Protocol.HTTP,
                Expires = DateTime.UtcNow.AddHours(_options.UrlExpirationDays),
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);

            _logger.LogTrace(
                "Bucket[{O0}]: Generated chunk upload url {O1} for multipart upload (id {O2}) for file {O3}",
                location.BucketName,
                partNumber,
                uploadId,
                location.FileId);

            return url;
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<Result<string, Error>> MultipartUploadStartAsync(
        string fileName,
        string contentType,
        FileLocation location,
        CancellationToken ct)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new InitiateMultipartUploadRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                ContentType = contentType,
                Metadata =
                {
                    ["file-name"] = fileName,
                },
            };

            var response = await _s3Client.InitiateMultipartUploadAsync(request, ct);

            _logger.LogTrace(
                "Bucket[{O0}]: Multipart upload started for file (id {O1}, name{O2}, contentType of {O3})",
                location.BucketName,
                location.FileId,
                fileName,
                contentType);

            return response.UploadId;
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<UnitResult<Error>> MultipartUploadAbortAsync(
        FileLocation location,
        string uploadId,
        CancellationToken ct)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new AbortMultipartUploadRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                UploadId = uploadId,
            };

            await _s3Client.AbortMultipartUploadAsync(request, ct);

            _logger.LogInformation(
                "Bucket[{O0}]: Multipart upload (id {O1}) aborted!",
                location.BucketName,
                uploadId);

            var partsUploading = await _s3Client.ListPartsAsync(location.BucketName, location.FileId, uploadId, ct);

            if (partsUploading.ContentLength > 0)
            {
                _logger.LogWarning(
                    "Bucket[{O0}]: Loose parts still uploading (uploadId {O1})! Cleanup might be needed",
                    location.BucketName,
                    uploadId);
            }

            return Result.Success<Error>();
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<Result<string, Error>> MultipartUploadCompleteAsync(
        FileLocation location,
        string uploadId,
        IEnumerable<Domain.FileManagment.PartETag> partETags,
        CancellationToken ct)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new CompleteMultipartUploadRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                UploadId = uploadId,
                PartETags = partETags
                    .Select(x => new Amazon.S3.Model.PartETag(x.PartNumber, x.ETag))
                    .ToList(),
            };

            var response = await _s3Client.CompleteMultipartUploadAsync(request, ct);

            _logger.LogInformation(
                "Bucket[{O0}]: Multipart upload (id {O1}) completed!",
                location.BucketName,
                uploadId);

            return response.Location;
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<Result<List<string>, Error>> ListBucketsAsync(CancellationToken ct)
    {
        try
        {
            var response = await _s3Client.ListBucketsAsync(ct);
            return response.Buckets.Select(b => b.BucketName).ToList();
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception);
        }
    }

    public async Task<UnitResult<Error>> UploadFileAsync(
        FileLocation location,
        string? contentType,
        Stream file,
        CancellationToken ct)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new PutObjectRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                InputStream = file,
                ContentType = contentType ?? "application/octet-stream",
            };

            await _s3Client.PutObjectAsync(request, ct);
            return Result.Success<Error>();
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<Result<string, Error>> DeleteFileAsync(
        FileLocation location,
        CancellationToken ct)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var result = await _s3Client.DeleteObjectAsync(
                location.BucketName,
                location.FileId,
                cancellationToken: ct);

            _logger.LogInformation(
                "Bucket[{O0}]: File (id {O1}) deleted!",
                location.BucketName,
                location.FileId);

            return location.FileId;
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<Result<FileUrl, Error>> CreatePresignedDownloadUrlAsync(
        FileLocation location,
        int expirationHours)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                Verb = HttpVerb.GET,
                Protocol = Protocol.HTTP,
                Expires = DateTime.UtcNow.AddHours(expirationHours),
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);

            _logger.LogTrace(
                "Bucket[{O0}]: Generated download url for file (id {O1})",
                location.BucketName,
                location.FileId);

            return new FileUrl(location.FileId, url);
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    public async Task<Result<string, Error>> CreatePresignedUploadUrlAsync(
        string fileName,
        FileLocation location)
    {
        try
        {
            await _s3Client.EnsureBucketExistsAsync(location.BucketName);

            var request = new GetPreSignedUrlRequest
            {
                BucketName = location.BucketName,
                Key = location.FileId,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddDays(_options.UrlExpirationDays),
                Protocol = Protocol.HTTP,
                ContentType = "application/octet-stream",
                Metadata =
                {
                    ["file-name"] = fileName,
                },
            };

            var url = await _s3Client.GetPreSignedURLAsync(request);

            _logger.LogTrace(
                "Bucket[{O0}]: Generated upload url for file (id {O1}, name{O2})",
                location.BucketName,
                location.FileId,
                fileName);

            return url;
        }
        catch (AmazonS3Exception exception)
        {
            return ErrorResult(exception, location.BucketName);
        }
    }

    private Error ErrorResult(AmazonS3Exception exception, string bucket = "unknown", string code = "amazonS3.failure")
    {
        _logger.LogError(exception, "Bucket[{O0}]: S3Provider error!", bucket);
        return Error.Failure(code, exception.Message);
    }
}