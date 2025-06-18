using ProjectPet.FileService.Domain.FileManagment;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace ProjectPet.FileService.Features;

public static class PresignedUrlUpload
{
    private record PresignedUrlUploadRequest(FileLocation FileLocation, string FileName, string? ContentType);

    private record PresignedUrlUploadDto(string Url);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapPost("/api/files/urlupload", Handler);
    }

    private static async Task<IResult> Handler(
        PresignedUrlUploadRequest request,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        var s3Result = await amazonS3.CreatePresignedUploadUrlAsync(
                request.FileName,
                request.FileLocation,
                ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new PresignedUrlUploadDto(s3Result.Value);

        return Results.Ok(response);
    }
}
