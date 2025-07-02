namespace ProjectPet.FileService.Contracts.Features.MultipartStartUpload;

public record MultipartStartUploadRequest(string FileName, string ContentType, long FileSizeBytes, string BucketName);
