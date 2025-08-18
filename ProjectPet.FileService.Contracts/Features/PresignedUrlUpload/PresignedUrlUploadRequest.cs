using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.PresignedUrlUpload;

public record PresignedUrlUploadRequest(FileLocationDto FileLocation, string FileName, string? ContentType);
