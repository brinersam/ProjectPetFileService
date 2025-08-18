namespace ProjectPet.FileService.Contracts.Features.MultipartStartUpload;

public record MultipartStartUploadRequest(string FileName, string ContentType, long FileSize, string BucketName);
