using Hangfire;
using ProjectPet.FileService.Jobs;

namespace ProjectPet.FileService.Infrastructure.Hangfire;

public static class StartupJobs
{
    public static IApplicationBuilder EnqueueStartupJobs(this IApplicationBuilder app)
    {
        BackgroundJob.Enqueue<TimeoutStuckUploadsJob>((x) => x.EnqueueSelf());
        return app;
    }
}
