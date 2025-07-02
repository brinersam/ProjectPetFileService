using Microsoft.Extensions.Options;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Contracts.Features.MultipartStartUpload;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using ProjectPet.FileService.Options;
using ProjectPet.SharedKernel.ErrorClasses;

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
        int chunkSizeMb = options.Value.ChunkSizeMb;

        string fileId = Guid.NewGuid().ToString();

        int totalChunks = (int)Math.Ceiling(((double)request.FileSizeBytes / 1024 / 1024) / (double)chunkSizeMb);
        if (totalChunks > 9)
            return Results.BadRequest(Error.Failure("too.many.chunks", $"Size of the file is too big! Almost created {totalChunks} chunks"));

        FileLocationDto location = new(fileId, request.BucketName);

        var s3Result = await amazonS3.MultipartUploadStartAsync(
            request.FileName,
            request.ContentType,
            location,
            ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new MultipartStartUploadResponse(
                new FileLocationDto(fileId, request.BucketName),
                s3Result.Value,
                chunkSizeMb,
                totalChunks);

        return Results.Ok(response);
    }
}
