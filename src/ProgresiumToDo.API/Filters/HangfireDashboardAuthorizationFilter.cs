using Hangfire.Dashboard;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ProgresiumToDo.API.Filters;

public class HangfireDashboardAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _environment;

    public HangfireDashboardAuthorizationFilter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public bool Authorize(DashboardContext context)
    {
        return _environment.IsDevelopment();
    }
}
