using System.Diagnostics.CodeAnalysis;
using Hangfire.Dashboard;

namespace ProjectPet.FileService.Infrastructure.Hangfire;

public class HangFireDisabledAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize([NotNull] DashboardContext context)
    {
        return true;
    }
}