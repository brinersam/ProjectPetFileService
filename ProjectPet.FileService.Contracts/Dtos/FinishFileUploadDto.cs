namespace ProjectPet.FileService.Contracts.Dtos;

public record FinishFileUploadDto(
    FileLocationDto Location,
    string UploadId,
    PartETagDto[] Etags)
{ }
