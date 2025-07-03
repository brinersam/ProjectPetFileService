namespace ProjectPet.FileService.Options;

public class TimeoutStuckUploadsJobOptions
{
    public static readonly string SECTION = "Job_TimeoutStuckUploads";

    public int UploadTimeoutMin { get; init; } = 30;

    public int UploadTimeoutCheckIntervalMin { get; init; } = 10;
}