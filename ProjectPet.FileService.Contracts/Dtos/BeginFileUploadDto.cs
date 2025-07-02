namespace ProjectPet.FileService.Contracts.Dtos;

public record BeginFileUploadDto(
    string BucketName,
    string FileName,
    long SizeBytes,
    string ContentType)
{ }
