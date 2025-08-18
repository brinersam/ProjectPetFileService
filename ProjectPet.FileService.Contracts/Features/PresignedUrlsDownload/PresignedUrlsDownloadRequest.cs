using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.PresignedUrlsDownload;

public record PresignedUrlsDownloadRequest(List<FileLocationDto> FileLocations);
