using FluentValidation;
using ProjectPet.FileService.Domain.FileManagment;
using ProjectPet.FileService.Endpoints;
using ProjectPet.FileService.Infrastructure.Providers;
using IResult = Microsoft.AspNetCore.Http.IResult;

namespace ProjectPet.FileService.Features;

public static class PresignedUrlsDownload
{
    private record PresignedUrlsDownloadRequest(List<FileLocation> FileLocations);

    private record PresignedUrlsDownloadDto(List<FileUrl> Urls);

    private class PresignedUrlsDownloadRequestValidator : AbstractValidator<PresignedUrlsDownloadRequest>
    {
        public PresignedUrlsDownloadRequestValidator()
        {
            RuleFor(x => x.FileLocations).NotEmpty();
        }
    }

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
            => app.MapPost("/api/files/urlsdownload", Handler);
    }

    private static async Task<IResult> Handler(
        PresignedUrlsDownloadRequest request,
        IS3Provider amazonS3)
    {
        var validator = new PresignedUrlsDownloadRequestValidator();
        var validatorResult = validator.Validate(request);
        if (validatorResult.IsValid == false)
            return Results.BadRequest(validatorResult.Errors);

        var result = await Task.WhenAll(
            request.FileLocations.Select(x => amazonS3.CreatePresignedDownloadUrlAsync(x, 1)));

        if (result.Any(x => x.IsFailure))
            return Results.BadRequest(String.Join(";", result.Where(x => x.IsFailure).Select(x => x.Error)));

        var response = new PresignedUrlsDownloadDto(result.Select(x => x.Value).ToList());

        return Results.Ok(response);
    }
}
