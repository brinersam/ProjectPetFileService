using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.MultipartStartUpload;

public record MultipartStartUploadResponse(FileLocationDto location, string UploadId, long ChunkSize, int TotalChunks);