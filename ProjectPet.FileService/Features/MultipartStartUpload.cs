using Microsoft.Extensions.Options;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Contracts.Features.MultipartStartUpload;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using ProjectPet.FileService.Options;

namespace ProjectPet.FileService.Features;

public static class MultipartStartUpload
{
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

        FileLocationDto location = new(fileId, request.BucketName);

        var s3Result = await amazonS3.MultipartUploadStartAsync(
            $"{location.BucketName}/{location.FileId}",
            request.ContentType,
            location,
            ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new MultipartStartUploadResponse(
                fileId,
                s3Result.Value,
                request.BucketName,
                chunkSizeMb,
                totalChunks);

        return Results.Ok(response);
    }
}
