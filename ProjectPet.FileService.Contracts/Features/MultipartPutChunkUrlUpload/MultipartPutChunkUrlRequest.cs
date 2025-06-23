using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Contracts.Features.MultipartPutChunkUrlUpload;

public record MultipartPutChunkUrlRequest(FileLocationDto FileLocation, string UploadId, int PartNumber);
