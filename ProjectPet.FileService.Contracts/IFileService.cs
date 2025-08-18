using CSharpFunctionalExtensions;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.FileService.Contracts.Features.DeleteFile;
using ProjectPet.FileService.Contracts.Features.MultipartCancelUpload;
using ProjectPet.FileService.Contracts.Features.MultipartFinishUpload;
using ProjectPet.FileService.Contracts.Features.MultipartPutChunkUrlUpload;
using ProjectPet.FileService.Contracts.Features.MultipartStartUpload;
using ProjectPet.FileService.Contracts.Features.PresignedUrlsDownload;
using ProjectPet.FileService.Contracts.Features.PresignedUrlUpload;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Contracts;

public interface IFileService
{
    Task<Result<DeleteFileResponse, Error>> DeleteFileAsync(FileLocationDto request, CancellationToken ct = default);

    Task<Result<MultipartFinishUploadResponse, Error>> MultipartFinishUploadAsync(MultipartFinishUploadRequest request, CancellationToken ct = default);

    Task<Result<MultipartPutChunkUrlResponse, Error>> MultipartPutChunkUrlUploadAsync(MultipartPutChunkUrlRequest request, CancellationToken ct = default);

    Task<Result<MultipartStartUploadResponse, Error>> MultipartStartUpload(MultipartStartUploadRequest request, CancellationToken ct = default);

    Task<Result<PresignedUrlsDownloadResponse, Error>> PresignedUrlsDownloadAsync(PresignedUrlsDownloadRequest request, CancellationToken ct = default);

    Task<Result<PresignedUrlUploadResponse, Error>> PresignedUrlUploadAsync(PresignedUrlUploadRequest request, CancellationToken ct = default);

    Task<UnitResult<Error>> MultipartCancelUploadAsync(MultipartCancelUploadRequest request, CancellationToken ct = default);
}