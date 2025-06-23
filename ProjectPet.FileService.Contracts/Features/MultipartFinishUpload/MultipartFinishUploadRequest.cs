using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Domain.FileManagment;

namespace ProjectPet.FileService.Contracts.Features.MultipartFinishUpload;
public record MultipartFinishUploadRequest(FileLocationDto FileLocation, string UploadId, List<PartETagDto> PartEtags);
