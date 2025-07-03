using FluentValidation;
using ProjectPet.FileService.Contracts.Features.MultipartPutChunkUrlUpload;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.AmazonS3;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace ProjectPet.FileService.Features;

public static class MultipartPutChunkUrlUpload
{
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

        var response = new MultipartPutChunkUrlResponse(s3Result.Value, request.PartNumber);

        return Results.Ok(response);
    }
}
