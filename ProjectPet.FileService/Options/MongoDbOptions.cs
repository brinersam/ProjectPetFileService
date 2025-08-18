namespace ProjectPet.FileService.Options;

public class MongoDbOptions
{
    public static readonly string SECTION = "MongoDbOptions";

    public string Endpoint { get; init; } = String.Empty;

    public string JobDBName { get; init; } = String.Empty;
}
