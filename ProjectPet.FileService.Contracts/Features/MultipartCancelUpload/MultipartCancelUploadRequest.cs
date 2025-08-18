using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.MultipartCancelUpload;
public record MultipartCancelUploadRequest(FileLocationDto FileLocation, string UploadId);