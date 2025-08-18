using FluentValidation;
using ProjectPet.FileService.Contracts.Features.MultipartFinishUpload;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace ProjectPet.FileService.Features;

public static class MultipartFinishUpload
{
    private class MultipartFinishUploadRequestValidator : AbstractValidator<MultipartFinishUploadRequest>
    {
        public MultipartFinishUploadRequestValidator()
        {
            RuleFor(x => x.PartEtags).NotEmpty();
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapPost("/api/files/multipart/finish", Handler);
    }

    private static async Task<IResult> Handler(
        MultipartFinishUploadRequest request,
        IS3Provider amazonS3,
        CancellationToken ct)
    {
        var validator = new MultipartFinishUploadRequestValidator();
        var validatorResult = validator.Validate(request);
        if (validatorResult.IsValid == false)
            return Results.BadRequest(validatorResult.Errors);

        var s3Result = await amazonS3.MultipartUploadCompleteAsync(
                request.FileLocation,
                request.UploadId,
                request.PartEtags,
                ct);

        if (s3Result.IsFailure)
            return Results.BadRequest(s3Result.Error);

        var response = new MultipartFinishUploadResponse(s3Result.Value);

        return Results.Ok(response);
    }
}
