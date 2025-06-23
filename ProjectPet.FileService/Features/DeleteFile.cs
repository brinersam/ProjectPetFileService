using Microsoft.AspNetCore.Mvc;
using ProjectPet.FileService.Contracts.Features.DeleteFile;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;

namespace ProjectPet.FileService.Features;

public static class DeleteFile
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapDelete("/api/files/{id:guid}/delete", Handler);
    }

    private static async Task<IResult> Handler(
        [FromRoute] Guid id,
        [FromQuery] string bucket,
        [FromBody] DeleteFileRequest request,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        var s3Result = await amazonS3.DeleteFileAsync(request.FileLocation, ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new DeleteFileResponse(s3Result.Value);

        return Results.Ok(response);
    }
}
