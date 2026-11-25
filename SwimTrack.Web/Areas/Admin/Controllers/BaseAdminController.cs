using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SwimTrack.Web.Controllers;
using static SwimTrack.Shared.Constants.ApplicationDefaults.Identity;

namespace SwimTrack.Web.Areas.Admin.Controllers;

[Area(Role.Admin)]
[Authorize(Roles = Role.Admin)]
public abstract class BaseAdminController : BaseController
{
}
