using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.Web.ViewModels;

namespace SwimTrack.Web.Controllers;

[AllowAnonymous]
public class HomeController : BaseController
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated ?? false)
        {
            return RedirectToAction(nameof(TrainingPlanController.All), "TrainingPlan");
        }

        return View();
    }

    [IgnoreAntiforgeryToken]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error(int? statusCode)
    {
        ErrorViewModel viewModel = new()
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
        };

        int resolvedStatusCode = statusCode
            ?? HttpContext.Response.StatusCode;

        return resolvedStatusCode switch
        {
            StatusCodes.Status400BadRequest => View("BadRequest", viewModel),
            StatusCodes.Status404NotFound => View("NotFound"),
            StatusCodes.Status500InternalServerError => View("InternalServerError", viewModel),
            _ => View(viewModel),
        };
    }
}
