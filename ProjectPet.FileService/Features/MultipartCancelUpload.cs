using ProjectPet.FileService.Contracts.Features.MultipartCancelUpload;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;

namespace ProjectPet.FileService.Features;

public static class MultipartCancelUpload
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapPost("/api/files/multipart/cancel", Handler);
    }

    private static async Task<IResult> Handler(
        MultipartCancelUploadRequest request,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        var s3Result = await amazonS3.MultipartUploadAbortAsync(
            request.FileLocation,
            request.UploadId,
            ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        return Results.Ok();
    }
}
