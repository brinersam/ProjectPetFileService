using CSharpFunctionalExtensions;
using ProjectPet.FileService.Domain.FileManagment;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Infrastructure.Providers;
public interface IS3Provider
{
    Task<Result<FileUrl, Error>> CreatePresignedDownloadUrlAsync(FileLocation location, int expirationHours);

    Task<Result<string, Error>> CreatePresignedUploadUrlAsync(FileLocation location, string uploadId, int partNumber);

    Task<Result<string, Error>> CreatePresignedUploadUrlAsync(string fileName, FileLocation location);

    Task<Result<string, Error>> DeleteFileAsync(FileLocation location, CancellationToken ct);

    Task<Result<List<string>, Error>> ListBucketsAsync(CancellationToken ct);

    Task<UnitResult<Error>> MultipartUploadAbortAsync(FileLocation location, string uploadId, CancellationToken ct);

    Task<Result<string, Error>> MultipartUploadCompleteAsync(FileLocation location, string uploadId, IEnumerable<PartETag> partETags, CancellationToken ct);

    Task<Result<string, Error>> MultipartUploadStartAsync(string fileName, string contentType, FileLocation location, CancellationToken ct);

    Task<UnitResult<Error>> UploadFileAsync(FileLocation location, string? contentType, Stream file, CancellationToken ct);

}