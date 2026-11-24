using Microsoft.AspNetCore.Http;

using static SwimTrack.Shared.Constants.ApplicationDefaults;
using static SwimTrack.Shared.Constants.ApplicationDefaults.Identity.Role;

namespace SwimTrack.Web.Infrastructure.Middlewares;

public class AdminRedirectionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly string _redirectPath;

    public AdminRedirectionMiddleware(RequestDelegate next, string redirectPath)
    {
        _next = next;
        _redirectPath = redirectPath;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path == ApplicationHomePagePath)
        {
            bool isAuthenticated = context.User.Identity?.IsAuthenticated ?? false;
            if (isAuthenticated &&
                context.User.IsInRole(Admin))
            {
                context.Response.Redirect(_redirectPath);

                return;
            }
        }

        await _next.Invoke(context);
    }
}
