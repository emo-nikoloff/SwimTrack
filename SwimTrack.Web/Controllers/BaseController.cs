using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static SwimTrack.Shared.Constants.ValidationMessages.Identity;

namespace SwimTrack.Web.Controllers;

[Authorize]
public abstract class BaseController : Controller
{
    protected string UserId
        => ValidateAuthorizedUserId(User.FindFirstValue(ClaimTypes.NameIdentifier));

    private string ValidateAuthorizedUserId(string? userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException(InvalidUserId);
        }

        return userId;
    }
}
