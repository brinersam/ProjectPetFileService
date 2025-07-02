using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.MultipartFinishUpload;
public record MultipartFinishUploadRequest(FileLocationDto Location, string UploadId, List<PartETagDto> PartEtags);
