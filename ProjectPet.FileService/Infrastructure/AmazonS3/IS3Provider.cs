using CSharpFunctionalExtensions;
using ProjectPet.FileService.Contracts.Dtos;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Infrastructure.AmazonS3;
public interface IS3Provider
{
    Task<Result<FileUrlDto, Error>> CreatePresignedDownloadUrlAsync(FileLocationDto location, int expirationHours);

    Task<Result<string, Error>> CreatePresignedUploadUrlAsync(FileLocationDto location, string uploadId, int partNumber);

    Task<Result<string, Error>> CreatePresignedUploadUrlAsync(string fileName, FileLocationDto location);

    Task<Result<string, Error>> DeleteFileAsync(FileLocationDto location, CancellationToken ct);

    Task<Result<List<string>, Error>> ListBucketsAsync(CancellationToken ct);

    Task<UnitResult<Error>> MultipartUploadAbortAsync(FileLocationDto location, string uploadId, CancellationToken ct);

    Task<Result<FileLocationDto, Error>> MultipartUploadCompleteAsync(FileLocationDto location, string uploadId, IEnumerable<PartETagDto> partETags, CancellationToken ct);

    Task<Result<string, Error>> MultipartUploadStartAsync(string fileName, string contentType, FileLocationDto location, CancellationToken ct);

    Task<UnitResult<Error>> UploadFileAsync(FileLocationDto location, string? contentType, Stream file, CancellationToken ct);

}