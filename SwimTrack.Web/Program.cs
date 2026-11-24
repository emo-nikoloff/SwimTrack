using SwimTrack.Data.Extensions;
using SwimTrack.Services.Extensions;
using SwimTrack.Web.Infrastructure.Extensions;

namespace SwimTrack.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddApplicationDbContext(builder.Configuration);
        builder.Services.AddApplicationIdentity(builder.Configuration);
        builder.Services.AddApplicationRepositories();
        builder.Services.AddApplicationServices();
        builder.Services.AddControllersWithViews();
        builder.Services.AddRazorPages();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAdminHomePageRedirection("/Admin/Home");

        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}")
            .WithStaticAssets();
        app.MapRazorPages()
            .WithStaticAssets();

        app.UseDefaultRoles();
        app.UseAdminSeeding();

        app.Run();
    }
}
