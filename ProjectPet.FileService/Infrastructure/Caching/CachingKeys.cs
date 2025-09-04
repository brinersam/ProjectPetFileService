using ProjectPet.FileService.Contracts.Dtos;

namespace ProjectPet.FileService.Infrastructure.Caching;

public static class CachingKeys
{
    public static string GetKey(FileLocationDto fileDto)
        => $"{fileDto.FileId}:{fileDto.BucketName}";
}
