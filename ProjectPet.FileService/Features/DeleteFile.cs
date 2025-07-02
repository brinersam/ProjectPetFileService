using Microsoft.AspNetCore.Mvc;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Contracts.Features.DeleteFile;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;

namespace ProjectPet.FileService.Features;

public static class DeleteFile
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapDelete("/api/files/{fileId}/delete", Handler);
    }

    private static async Task<IResult> Handler(
        [FromRoute] string fileId,
        [FromQuery] string bucket,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        var s3Result = await amazonS3.DeleteFileAsync(new FileLocationDto(fileId, bucket), ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error.Message);

        var response = new DeleteFileResponse(s3Result.Value);

        return Results.Ok(response);
    }
}
