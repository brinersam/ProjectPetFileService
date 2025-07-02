using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Json;
using System.Web;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using ProjectPet.FileService.Contracts;
using ProjectPet.FileService.Contracts.Features.DeleteFile;
using ProjectPet.FileService.Contracts.Features.MultipartCancelUpload;
using ProjectPet.FileService.Contracts.Features.MultipartFinishUpload;
using ProjectPet.FileService.Contracts.Features.MultipartPutChunkUrlUpload;
using ProjectPet.FileService.Contracts.Features.MultipartStartUpload;
using ProjectPet.FileService.Contracts.Features.PresignedUrlsDownload;
using ProjectPet.FileService.Contracts.Features.PresignedUrlUpload;
using ProjectPet.SharedKernel.ErrorClasses;

namespace ProjectPet.FileService.Communication;
public class FileServiceClient : IFileService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public FileServiceClient(HttpClient httpClient, IOptions<FileServiceOptions> optionsObj)
    {
        var options = optionsObj.Value;

        _httpClient = httpClient;
        _baseUrl = options.Endpoint;
    }

    public async Task<Result<DeleteFileResponse, Error>> DeleteFileAsync(FileLocationDto location, CancellationToken ct = default)
    {
        var uri = BuildUri(
            $"api/files/{id}/delete",
            x => x["bucket"] = bucket);

        var fileResponse = await response.Content.ReadFromJsonAsync<DeleteFileResponse>(ct);
        return fileResponse!;
    }

    public async Task<UnitResult<Error>> MultipartCancelUploadAsync(MultipartCancelUploadRequest request, CancellationToken ct = default)
    {
        var uri = BuildUri("/api/files/multipart/cancel");

        return await CallHttpClientAsync(uri, request, ct);
    }

    public async Task<Result<MultipartFinishUploadResponse, Error>> MultipartFinishUploadAsync(MultipartFinishUploadRequest request, CancellationToken ct = default)
    {
        var uri = BuildUri("/api/files/multipart/finish");

        return await CallHttpClientAsync<MultipartFinishUploadRequest, MultipartFinishUploadResponse>(uri, request, ct);
    }

    public async Task<Result<MultipartPutChunkUrlResponse, Error>> MultipartPutChunkUrlUploadAsync(MultipartPutChunkUrlRequest request, CancellationToken ct = default)
    {
        var uri = BuildUri("/api/files/multipart/url");

        return await CallHttpClientAsync<MultipartPutChunkUrlRequest, MultipartPutChunkUrlResponse>(uri, request, ct);
    }

    public async Task<Result<MultipartStartUploadResponse, Error>> MultipartStartUpload(MultipartStartUploadRequest request, CancellationToken ct = default)
    {
        var uri = BuildUri("/api/files/multipart/start");

        return await CallHttpClientAsync<MultipartStartUploadRequest, MultipartStartUploadResponse>(uri, request, ct);
    }

    public async Task<Result<PresignedUrlsDownloadResponse, Error>> PresignedUrlsDownloadAsync(PresignedUrlsDownloadRequest request, CancellationToken ct = default)
    {
        var uri = BuildUri("/api/files/urlsdownload");

        return await CallHttpClientAsync<PresignedUrlsDownloadRequest, PresignedUrlsDownloadResponse>(uri, request, ct);
    }

    public async Task<Result<PresignedUrlUploadResponse, Error>> PresignedUrlUploadAsync(PresignedUrlUploadRequest request, CancellationToken ct = default)
    {
        var uri = BuildUri("/api/files/urlupload");

        return await CallHttpClientAsync<PresignedUrlUploadRequest, PresignedUrlUploadResponse>(uri, request, ct);
    }

    private async Task<Result<TResponse, Error>> CallHttpClientAsync<TRequest, TResponse>(string uri, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, request, ct);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            return Error.Failure("fileservice.error", error);
        }

        var fileResponse = await response.Content.ReadFromJsonAsync<TResponse>(ct);
        return fileResponse!;
    }

    private async Task<UnitResult<Error>> CallHttpClientAsync<TRequest>(string uri, TRequest request, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync(uri, request, ct);

        if (response.StatusCode != HttpStatusCode.OK)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            return Error.Failure("fileservice.error", error);
        }

        return Result.Success<Error>();
    }

    private string BuildUri(string endPoint, Action<NameValueCollection> queryOpts = null!)
    {
        var baseUri = _baseUrl.TrimEnd('/');
        var specificUri = endPoint.TrimStart('/');

        var builder = new UriBuilder(string.Format("{0}/{1}", baseUri, specificUri));
        builder.Port = -1;

        if (queryOpts is not null)
        {
            var query = HttpUtility.ParseQueryString(builder.Query);
            queryOpts?.Invoke(query);

            // query["foo"] = "bar<>&-baz";
            // query["bar"] = "bazinga";
            // foo=bar%3c%3e%26-baz&bar=bazinga
            builder.Query = query.ToString();
        }

        return builder.ToString();

        // http://example.com/?foo=bar%3c%3e%26-baz&bar=bazinga
    }
}
