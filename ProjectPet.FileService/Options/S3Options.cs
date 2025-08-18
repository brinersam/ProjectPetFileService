namespace ProjectPet.FileService.Options;

public class S3Options
{
    public static readonly string SECTION = "S3Options";

    public string Endpoint { get; init; } = String.Empty;

    public string AccessKey { get; init; } = String.Empty;

    public string SecretKey { get; init; } = String.Empty;

    public bool WithSsl { get; init; }

    public double UrlExpirationDays { get; init; }

    public int ChunkSizeMb { get; init; }

}
