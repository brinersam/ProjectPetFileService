using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.MultipartFinishUpload;
public record MultipartFinishUploadRequest(FileLocationDto FileLocation, string UploadId, List<PartETagDto> PartEtags);
