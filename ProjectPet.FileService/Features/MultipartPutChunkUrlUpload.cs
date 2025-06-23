using FluentValidation;
using ProjectPet.FileService.Domain.FileManagment;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace ProjectPet.FileService.Features;

public static class MultipartPutChunkUrlUpload
{
    private record MultipartPutChunkUrlRequest(FileLocation FileLocation, string UploadId, int PartNumber);

    private record MultipartPutChunkUrlDto(string Url, int PartNumber);

    private class MultipartPutChunkUrlRequestValidator : AbstractValidator<MultipartPutChunkUrlRequest>
    {
        public MultipartPutChunkUrlRequestValidator()
        {
            RuleFor(x => x.PartNumber).GreaterThan(0);
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapPost("/api/files/multipart/url", Handler);
    }

    private static async Task<IResult> Handler(
        MultipartPutChunkUrlRequest request,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        var validator = new MultipartPutChunkUrlRequestValidator();
        var validatorResult = validator.Validate(request);
        if (validatorResult.IsValid == false)
            return Results.BadRequest(validatorResult.Errors);

        var s3Result = await amazonS3.CreatePresignedUploadUrlAsync(
                request.FileLocation,
                request.UploadId,
                request.PartNumber);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new MultipartPutChunkUrlDto(s3Result.Value, request.PartNumber);

        return Results.Ok(response);
    }
}
