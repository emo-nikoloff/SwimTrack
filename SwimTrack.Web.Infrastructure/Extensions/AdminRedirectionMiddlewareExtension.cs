using Microsoft.AspNetCore.Builder;

using SwimTrack.Web.Infrastructure.Middlewares;

namespace SwimTrack.Web.Infrastructure.Extensions;

public static class AdminRedirectionMiddlewareExtension
{
    public static IApplicationBuilder UseAdminHomePageRedirection(this IApplicationBuilder app, string adminHomePagePath)
    {
        app.UseMiddleware<AdminRedirectionMiddleware>(adminHomePagePath);

        return app;
    }
}
