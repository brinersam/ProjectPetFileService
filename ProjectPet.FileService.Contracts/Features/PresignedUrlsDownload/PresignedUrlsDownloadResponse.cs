using ProjectPet.FileService.Domain.FileManagment;

namespace ProjectPet.FileService.Contracts.Features.PresignedUrlsDownload;

public record PresignedUrlsDownloadResponse(List<FileUrlDto> Urls);