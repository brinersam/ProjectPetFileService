namespace ProjectPet.FileService.Contracts.Features.MultipartStartUpload;

public record MultipartStartUploadResponse(string FileId, string UploadId, string BucketName, long ChunkSize, int TotalChunks);