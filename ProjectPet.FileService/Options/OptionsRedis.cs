namespace ProjectPet.FileService.Options;

public class OptionsRedis
{
    public const string SECTION = "RedisOptions";
    public string Endpoint { get; init; } = "REDIS_CONNECTION_STRING";
}
