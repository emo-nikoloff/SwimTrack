using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static SwimTrack.Shared.Constants.ApplicationDefaults.Identity;

namespace SwimTrack.Web.Areas.Admin.Controllers;

[Area(Role.Admin)]
[Authorize(Roles = Role.Admin)]
[AutoValidateAntiforgeryToken]
public abstract class BaseAdminController : Controller
{
}
