using Hangfire;
using Microsoft.Extensions.Options;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Infrastructure.AmazonS3;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Jobs;

public class TimeoutStuckUploadsJob
{
    private readonly ILogger<TimeoutStuckUploadsJob> _logger;
    private readonly IS3Provider _s3Provider;
    private TimeoutStuckUploadsJobOptions _options;

    public TimeoutStuckUploadsJob(
        ILogger<TimeoutStuckUploadsJob> logger,
        IOptions<TimeoutStuckUploadsJobOptions> options,
        IS3Provider s3Provider)
    {
        _options = options.Value;
        _logger = logger;
        _s3Provider = s3Provider;
    }

    public void EnqueueSelf()
    {
        var intervalMin = _options.UploadTimeoutCheckIntervalMin;

        RecurringJob.AddOrUpdate<TimeoutStuckUploadsJob>(
                nameof(TimeoutStuckUploadsJob),
                x => x.RunAsync(CancellationToken.None), // .none will be replaced automatically
                $"*/{intervalMin} * * * *"
            );
    }

    public async Task RunAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("{jobname}: Starting upload cleanup!", nameof(TimeoutStuckUploadsJob));

        var bucketsRes = await _s3Provider.ListBucketsAsync(ct);
        if (bucketsRes.IsFailure)
        {
            _logger.LogError(
                "Failed to retrieve buckets!: {error}!",
                bucketsRes.Error);

            return;
        }

        await Task.WhenAll(bucketsRes.Value.Select(async bucketName =>
            {
                var uploadsList = await _s3Provider.ListMultipartUploadsAsync(bucketName, ct);
                if (uploadsList.IsFailure)
                {
                    _logger.LogError(
                        "Failed to retrieve uploads from a bucket {bucketName}: {error}!",
                        bucketName,
                        uploadsList.Error);
                    return;
                }

                await Task.WhenAll(uploadsList.Value.Select(async upload =>
                        {
                            if (upload.Initiated is null)
                                return;

                            var minutesSinceUploadBegan = DateTime.UtcNow.Subtract((DateTime)upload.Initiated)!.TotalMinutes;
                            if (minutesSinceUploadBegan > _options.UploadTimeoutMin)
                            {
                                _logger.LogInformation(
                                    "{jobname}: Cleaning up stuck upload with id {uploadId} for a file {fileId} on bucket {bucketname}...",
                                    nameof(TimeoutStuckUploadsJob),
                                    upload.UploadId,
                                    upload.Key,
                                    bucketName
                                );

                                await _s3Provider.MultipartUploadAbortAsync(
                                    new FileLocationDto(upload.Key, bucketName),
                                    upload.UploadId,
                                    ct
                                );

                                _logger.LogInformation(
                                    "{jobname}: Upload with id {uploadId} cleaned up successfully!",
                                    nameof(TimeoutStuckUploadsJob),
                                    upload.UploadId
                                );
                            }
                        })
                );
            })
        );

        _logger.LogInformation("{jobname}: Upload cleanup finished!", nameof(TimeoutStuckUploadsJob));
    }
}