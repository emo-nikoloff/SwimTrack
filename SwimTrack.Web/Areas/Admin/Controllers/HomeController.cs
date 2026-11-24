using Microsoft.AspNetCore.Mvc;

namespace SwimTrack.Web.Areas.Admin.Controllers;

public class HomeController : BaseAdminController
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
