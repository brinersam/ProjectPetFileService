using Microsoft.Extensions.Options;
using ProjectPet.FileService.Domain.FileManagment;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Features;

public static class MultipartStartUpload
{
    private record MultipartStartUploadRequest(string FileName, string ContentType, long FileSize, string BucketName);

    private record MultipartStartUploadDto(string FileId, string UploadId, string BucketName, long ChunkSize, int TotalChunks);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapPost("/api/files/multipart/start", Handler);
    }

    private static async Task<IResult> Handler(
        MultipartStartUploadRequest request,
        IOptions<S3Options> options,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        int chunkSizeMb = options.Value.ChunkSizeMb * 1024 * 1024;

        string fileId = Guid.NewGuid().ToString();

        int totalChunks = (int)Math.Ceiling((double)request.FileSize / (double)chunkSizeMb);

        FileLocation location = new(fileId, request.BucketName);

        var s3Result = await amazonS3.MultipartUploadStartAsync(
            $"{location.BucketName}/{location.FileId}",
            request.ContentType,
            location,
            ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new MultipartStartUploadDto(
                fileId,
                s3Result.Value,
                request.BucketName,
                chunkSizeMb,
                totalChunks);

        return Results.Ok(response);
    }
}
