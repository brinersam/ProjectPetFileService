namespace ProjectPet.FileService.Options;

public class S3Options
{
    public static readonly string SECTION = "S3Options";

    public string Endpoint { get; } = String.Empty;

    public string AccessKey { get; } = String.Empty;

    public string SecretKey { get; } = String.Empty;

    public bool WithSsl { get; }

    public double UrlExpirationDays { get; }

}
